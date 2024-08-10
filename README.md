# MassTransit Works

[Distributed Systems Challenges](https://github.com/buraksenyurt/DistributedChallenge) reposunda birden fazla sistemin birbirlerini kullandığı ortak bir süreç işletiliyor. Bu süreçte Rabbit MQ ve hatta Redis Streaming ile event yönetimi işletilmekte. Belli bir olay verisine ilişkin business nesne örneğinin çalıştırılmasında ise kendi yazdığım basit bir mekanizma çalıştırılıyor. [MassTransit](https://masstransit.io/) paketinin özellikle bu tür senaryolarda mesajları yönetmek, alıp vermek gibi açılardan daha kolay bir kullanım sağladığı ifade edilmekte. RabbitMQ ile ilgili kütüphaneleri kullanırken ele alınan bazı low-level sayılabilecek ayarlamalara da gerek olmadığından nispeten kolay bir kullanım sağlıyor. Hiç Masstransit ile çalışmadığımdan konuyu önce burada ele almayı ve sonra gerekli görürsem Distributed System Challenges reposuna entegre etmeyi planlıyorum.

## Senaryo

Distributed Systems Challenges gibi çok büyük bir senaryo ile çalışmak yerine MassTransit'i ele alacağımız nispeten daha basit bir senaryo söz konusu. Bir Quiz uygulamamız var. Yine kendi yazdığımız basit bir Authenticator servisini kullanıp JWT bazlı çalışıyor. Sisteme başarılı şekilde giriş yapmış olan kullanıcı karışık sırada gelen beş soruya cevap veriyor ve skor bilgisini içeren olay mesajı MassTransit aracılığı ile RabbitMQ'ya bırakılıyor. Hali hazırda var olan bir olay dinleyicisi de bu olayı alıp herhangibi repository'ye kaydediyor. Böylece oyuncunun bilgi yarışmasındaki puanlarını topladığımız bir istatistik bilgi bankasının oluşumunu mesajlaşma tabanlı bir sistem üstünden de görmüş oluyoruz. 

## AuthService

Basit login ve JWT token üreten servis için aşağıdaki curl komutları ile testler yapılabilir.

```bash
# Geçerli bir kullanıcı için
curl -X POST http://localhost:5280/login \
-H "Content-Type: application/json" \
-d '{"MemberName": "jhondoe", "Password": "123456"}'

# Geçersiz bir kullanıcı için
curl -X POST http://localhost:5280/login \
-H "Content-Type: application/json" \
-d '{"MemberName": "jacksparrow", "Password": "HiHiCaptain"}'
```