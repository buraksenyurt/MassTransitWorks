use chrono::{DateTime, Utc};
use lapin::options::{BasicPublishOptions, QueueDeclareOptions};
use lapin::types::FieldTable;
use lapin::Connection;
use lapin::{BasicProperties, ConnectionProperties};
use rand::prelude::SliceRandom;
use rand::thread_rng;
use serde::{Deserialize, Serialize};
use serde_json::{from_reader, to_string};
use std::fs::File;
use std::io::{stdin, stdout, Error, Write};

#[tokio::main]
async fn main() {
    let mut game = Game::default();

    println!("Dünyanın en popüler coğrafya quiz yarışmasına hoşgeldiniz.");
    println!("Sorular yükleniyor. Lütfen bir isim giriniz.");
    stdout().flush().unwrap();
    stdin()
        .read_line(&mut game.player_name)
        .expect("Okuma hatası.");
    println!("Yarışmamıza hoş geldin {}", game.player_name);
    let mut questions = load_questions("data.json").expect("Sorular okunamadı");
    run_quiz(&mut questions, &mut game);
    println!("Yarışma sona erdi. Toplam puanın...{}", game.total_score);

    if let Err(e) = send_message(game).await {
        eprintln!("Failed to send message to RabbitMQ: {}", e);
    }
}

#[derive(Default)]
struct Game {
    player_name: String,
    total_score: i32,
}

#[derive(Default, Deserialize)]
struct Question {
    question: String,
    answer: String,
    point: u8,
}

fn run_quiz(questions: &mut Vec<Question>, game: &mut Game) {
    let mut rng = thread_rng();
    questions.shuffle(&mut rng);
    for (idx, q) in questions.iter().enumerate() {
        let mut user_answer = String::new();
        println!("{idx} - {}", q.question);
        println!("Cevabın");
        stdout().flush().unwrap();
        stdin().read_line(&mut user_answer).expect("Okuma hatası");
        let user_answer = user_answer.trim();
        if user_answer.eq_ignore_ascii_case(&q.answer) {
            println!("Doğru cevap!\n");
            game.total_score += q.point as i32;
        } else {
            println!(
                "Üzgünüm ama yanlış cevap. Doğru cevap '{}' olacaktı.",
                q.answer
            );
            game.total_score -= 1;
        }
    }
}

fn load_questions(file_path: &str) -> Result<Vec<Question>, Error> {
    let file = File::open(file_path)?;
    let questions = from_reader(file)?;
    Ok(questions)
}

async fn send_message(game: Game) -> Result<(), Box<dyn std::error::Error>> {
    let new_game_score = NewGameScore::new(
        1,
        18,
        game.player_name.trim().to_string(),
        game.total_score as f64,
    );

    let payload = to_string(&new_game_score)?.into_bytes();

    let addr = "amqp://scothtiger:123456@localhost:5672/%2f";
    let conn = Connection::connect(addr, ConnectionProperties::default()).await?;
    let channel = conn.create_channel().await?;
    let _queue = channel
        .queue_declare(
            "msg_gamers_score",
            QueueDeclareOptions {
                durable: true,
                ..Default::default()
            },
            FieldTable::default(),
        )
        .await?;

    channel
        .basic_publish(
            "",
            "msg_gamers_score",
            BasicPublishOptions::default(),
            &payload,
            BasicProperties::default(),
        )
        .await?
        .await?;
    println!("Sent message: {:?}", new_game_score);

    Ok(())
}

#[derive(Serialize, Debug)]
struct NewGameScore {
    #[serde(rename = "PlayerID")]
    player_id: i32,
    #[serde(rename = "GameID")]
    game_id: i32,
    #[serde(rename = "Nickname")]
    nickname: String,
    #[serde(rename = "Point")]
    point: f64,
    #[serde(rename = "RecordTime")]
    record_time: DateTime<Utc>,
}

impl NewGameScore {
    fn new(player_id: i32, game_id: i32, nickname: String, point: f64) -> Self {
        NewGameScore {
            player_id,
            game_id,
            nickname,
            point,
            record_time: Utc::now(),
        }
    }
}
