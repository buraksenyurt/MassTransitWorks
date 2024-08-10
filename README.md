# MassTransit Works

[Distributed Systems Challenges](https://github.com/buraksenyurt/DistributedChallenge) reposunda birden fazla sistemin birbirlerini kullandığı ortak bir süreç işletiliyor. Bu süreçte Rabbit MQ ve hatta Redis Streaming ile event yönetimi işletilmekte. Belli bir olay verisine ilişkin business nesne örneğinin çalıştırılmasında ise kendi yazdığım basit bir mekanizma çalıştırılıyor. [MassTransit](https://masstransit.io/) paketinin özellikle bu tür senaryolarda mesajları yönetmek, alıp vermek gibi açılardan daha kolay bir kullanım sağladığı ifade edilmekte. RabbitMQ ile ilgili kütüphaneleri kullanırken ele alınan bazı low-level sayılabilecek ayarlamalara da gerek olmadığından nispeten kolay bir kullanım sağlıyor. Hiç Masstransit ile çalışmadığımdan konuyu önce burada ele almayı ve sonra gerekli görürsem Distributed System Challenges reposuna entegre etmeyi planlıyorum.

## Notlar

- contests-app isimli bir Rust uygulaması var. RabbitMQ'ya oyun skorunu mesaj olarak gönderebiliyor. Ancak ConsumerApp ile bu mesajları dinleyemiyorum. Mesajlar serileştirme hataları nedeniyle msg_gamers_score_error kuyruğuna düşüyor. ConsumerApp tarafındaki MassTransit hizmeti kendi istediği formatta bir paylod beklediği için bu hatanın oluştuğunu anlıyoruz.
