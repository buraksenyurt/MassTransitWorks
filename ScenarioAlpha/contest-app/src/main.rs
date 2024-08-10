use rand::prelude::SliceRandom;
use rand::thread_rng;
use std::io::{stdin, stdout, Write};

fn main() {
    let mut game = Game::default();

    println!("Dünyanın en popüler coğrafya quiz yarışmasına hoşgeldiniz.");
    println!("Sorular yükleniyor. Lütfen bir isim giriniz.");
    stdout().flush().unwrap();
    stdin()
        .read_line(&mut game.player_name)
        .expect("Okuma hatası.");
    println!("Yarışmamız hoş geldin {}", game.player_name);
    let mut questions = load_questions();
    run_quiz(&mut questions, &mut game);
    println!("Yarışma sona erdi. Toplam puanın...{}", game.total_score);
}

#[derive(Default)]
struct Game {
    player_name: String,
    total_score: i32,
}

#[derive(Default)]
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

fn load_questions() -> Vec<Question> {
    vec![
        Question {
            question: String::from("2024 Olimpiyat oyunları hangi şehirde yapılıyor?"),
            answer: String::from("Paris"),
            point: 3,
        },
        Question {
            question: String::from("Dünyanın en kalabalık ülkesidir."),
            answer: String::from("Çin"),
            point: 5,
        },
        Question {
            question: String::from("Dünyanın en uzun nehridir."),
            answer: String::from("Nil"),
            point: 7,
        },
        Question {
            question: String::from("Dünyanın en büyük çölü Sahra'nın bulunduğu kıtadı."),
            answer: String::from("Afrika"),
            point: 9,
        },
        Question {
            question: String::from("Japonya'nın başkentidir."),
            answer: String::from("Tokyo"),
            point: 3,
        },
        Question {
            question: String::from("Dünyanın en büyük okyanusudur."),
            answer: String::from("Pasifik"),
            point: 6,
        },
        Question {
            question: String::from("Dünyanın en çok doğal gölü bulunan ülkesidir."),
            answer: String::from("Kanada"),
            point: 9,
        },
        Question {
            question: String::from("Dünyanın yüz ölçümü açısından en küçük ülkesidir."),
            answer: String::from("Vatikan"),
            point: 6,
        },
        Question {
            question: String::from("Petra Antik Kentinin bulunduğu ülkedir"),
            answer: String::from("Ürdün"),
            point: 10,
        },
    ]
}
