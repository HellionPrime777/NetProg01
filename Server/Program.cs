
using CommonLibrary;
using Server;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

//Server


Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);

IPAddress ip = IPAddress.Parse("127.0.0.1");

IPEndPoint ep = new IPEndPoint(ip, 5000); // 5000 - port

s.Bind(ep);
s.Listen(1000);

Console.WriteLine("Server");

var core = new Core();

Action<Socket> worker = (ns) =>
{
    var buffer = new byte[4096];
    int read = ns.Receive(buffer);

    string incoming = Encoding.UTF8.GetString(buffer, 0, read);
    Console.WriteLine(incoming);
    var request = JsonSerializer.Deserialize<Data>(incoming);
    var response = JsonSerializer.Serialize(core.Handle(request));
    Console.WriteLine(response);
    ns.Send(Encoding.UTF8.GetBytes(response));
    ns.Shutdown(SocketShutdown.Both);
    ns.Close();
};



while (true)
{
    Task.Run(() => worker(s.Accept()));
}

/*
 * 
 * 1. Доробити код сервера, щоб він відповідав 
 * на повідомлення клієнта текстом - "Прийнято"
 * а код клієнта виводив те що прислав у відповідь сервер
 
 * 
 * 2. Доробити код клієнта щоб можна було писати повідомлення 
 * в консолі і вони передавалися на сервер
 * 
 * 
 * 3. Кліент-серверний чат
 * 
 * Клієнт
 * Коли клієнт під'єднується то першим передає своє ім'я
 * яке користувач вводить з клавіатури
 * Після цього, клієнт отримує від сервера повідомлення які прийшли 
 * на ім'я цього клієнта та виводить їх в консоль
 * Після цього клієнт може ввести ім'я та повідомлення для іншого
 * і відправити його на сервер.
 * На цьому робота клієнта завершується.
 * 
 * Сервер
 * Приймає ім'я клієнта. Якщо є для нього повідомлення
 * то віддає йому їх у вигляді масиву повідомлень
 * 
 * 
 * Після приймає повідомлення від клієнта
 * В повідомленні є ім'я для кого воно, від кого воно, текст 
 * та дата-час коли повідомлення створено
 * Сервер зберігає повідомлення в якомусь масиві
 * для інших користувачів
 * 
 * на цьому з'єднання з клієнтом завершується
 * 
 * 
 * 
 * 
 * 
 */ 