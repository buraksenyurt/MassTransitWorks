# MassTransit Works

[Distributed Systems Challenges](https://github.com/buraksenyurt/DistributedChallenge) reposunda birden fazla sistemin birbirlerini kullandığı ortak bir süreç işletiliyor. Bu süreçte Rabbit MQ ve hatta Redis Streaming ile event yönetimi işletilmekte. Belli bir olay verisine ilişkin business nesne örneğinin çalıştırılmasında ise kendi yazdığım basit bir mekanizma çalıştırılıyor. [MassTransit](https://masstransit.io/) paketinin özellikle bu tür senaryolarda mesajları yönetmek, alıp vermek gibi açılardan daha kolay bir kullanım sağladığı ifade etmekte. Hiç Masstransit ile çalışmadığımdan konuyu önce burada ele almayı sonra gerekli görürsem Distributed System Challenges reposuna entegre etmeyi planlıyorum.

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