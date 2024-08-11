# MassTransit Works

[Distributed Systems Challenges](https://github.com/buraksenyurt/DistributedChallenge) reposunda birden fazla sistemin birbirlerini kullandığı ortak bir süreç işletiliyor. Bu süreçte Rabbit MQ ve hatta Redis Streaming ile event yönetimi işletilmekte. Belli bir olay verisine ilişkin business nesne örneğinin çalıştırılmasında ise kendi yazdığım basit bir mekanizma çalıştırılıyor. [MassTransit](https://masstransit.io/) paketinin özellikle bu tür senaryolarda mesajları yönetmek, alıp vermek gibi açılardan daha kolay bir kullanım sağladığı ifade edilmekte. RabbitMQ ile ilgili kütüphaneleri kullanırken ele alınan bazı low-level sayılabilecek ayarlamalara da gerek olmadığından nispeten kolay bir kullanım sağlıyor. Hiç Masstransit ile çalışmadığımdan konuyu önce burada ele almayı ve sonra gerekli görürsem Distributed System Challenges reposuna entegre etmeyi planlıyorum.

## Senaryo

MassTransit kullanımı için ScenarioAlpha isimli solution kullanılabilir. ConsumerApp çalıştırıldıktan sonra testler için ProducerApi veya GameApp uygulamalarından yararlanılabilir. GameApp basit bir console oyunu. Coğrafya bilgi yarışması şeklinde. Çalışmasını bitirdikten sonra oyuncu için toplam skor bilgisini RabbitMQ'ya göndermekte ve aracı olarak MassTransit'i kullanmakta. ConsumerApp tahmin edileceği üzere MassTransit aracısını kullanarak RabbitMQ mesajlarını dinlemek üzere tasarlandı. ProducerApi ise sağladığı Post metodu ile yine aynı skor bilgisi mesajını RabbitMQ'ya göndermek üzere MassTransit'ten yararlanıyor.

## Notlar

- contests-app isimli bir Rust uygulaması var. RabbitMQ'ya oyun skorunu mesaj olarak gönderebiliyor. Ancak ConsumerApp ile bu mesajları dinleyemiyorum. Mesajlar serileştirme hataları nedeniyle msg_gamers_score_error kuyruğuna düşüyor. ConsumerApp tarafındaki MassTransit hizmeti kendi istediği formatta bir paylod beklediği için bu hatanın oluştuğunu anlıyoruz.
