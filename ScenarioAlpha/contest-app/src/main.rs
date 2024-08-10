use rand::prelude::SliceRandom;
use rand::thread_rng;
use serde::Deserialize;
use serde_json::from_reader;
use std::fs::File;
use std::io::{stdin, stdout, Error, Write};

fn main() {
    let mut game = Game::default();

    println!("Dünyanın en popüler coğrafya quiz yarışmasına hoşgeldiniz.");
    println!("Sorular yükleniyor. Lütfen bir isim giriniz.");
    stdout().flush().unwrap();
    stdin()
        .read_line(&mut game.player_name)
        .expect("Okuma hatası.");
    println!("Yarışmamız hoş geldin {}", game.player_name);
    let mut questions = load_questions("data.json").expect("Sorular okunamadı");
    run_quiz(&mut questions, &mut game);
    println!("Yarışma sona erdi. Toplam puanın...{}", game.total_score);
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
