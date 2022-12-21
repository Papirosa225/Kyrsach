//Разработка программы учета сведений об игроках хоккейной команды.
//	Сведения об игроках хоккейной команды включают: Ф.И.О.игрока, дату рождения, количество сыгранных матчей
//	число заброшенных шайб, количество голевых передач, количество штрафных минут.
//	Индивидуальное задание: вывести 6 лучших игроков(голы+передачи) с указанием их результативности.
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

const string path= @"E:\Курсач\Курсач\autorizationData.txt";
const string playerPath = @"E:\Курсач\Курсач\playersData.txt";
static string HashPassword(string password)
{
    if (password == null)  throw new ArgumentNullException("password");
    byte[] salt;
    byte[] buffer2;
    using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password, 0x10, 0x3e8))
    {
        salt = bytes.Salt;
        buffer2 = bytes.GetBytes(0x20);
    }
    byte[] dst = new byte[0x31];
    Buffer.BlockCopy(salt, 0, dst, 1, 0x10);
    Buffer.BlockCopy(buffer2, 0, dst, 0x11, 0x20);
    return Convert.ToBase64String(dst);
}
static bool VerifyHashedPassword(string hashedPassword, string password)
{
    byte[] buffer4;
    if (hashedPassword == null) return false;
    if (password == null) throw new ArgumentNullException("password");
    byte[] src = Convert.FromBase64String(hashedPassword);
    if ((src.Length != 0x31) || (src[0] != 0)) return false;
    byte[] dst = new byte[0x10];
    Buffer.BlockCopy(src, 1, dst, 0, 0x10);
    byte[] buffer3 = new byte[0x20];
    Buffer.BlockCopy(src, 0x11, buffer3, 0, 0x20);
    using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password, dst, 0x3e8))
    {
        buffer4 = bytes.GetBytes(0x20);
    }
    return buffer3.SequenceEqual(buffer4);
}
static void ShowInfo(string[,] FormedData,int Count)
{
    for (int i = 0; i < Count; i++)
    {
        Console.WriteLine($"Логин:{FormedData[i, 0]}\nПароль:{FormedData[i, 1]}\nПрава:{FormedData[i, 2]}\n");
    }
}
static void ShowListInfo(List<Player> players)
{
    foreach (var player in players)
    {
        Console.WriteLine($"\nФИО {player.name}\nДата рождения {player.birth}\nКол-во матчей {player.matchCount}\nКол-во передач {player.assistCount}\nКол-во штрафных минут {player.penaltyMinutes}\nКол-во голов {player.goalCount}\n");
    }
}
static void ArrayResize(int AccountCount, ref string[,] OldArray,int mode )
{
    if (mode == -1)
    {
        var NewArray = new string[AccountCount, 3];
        for (int i = 0; i < AccountCount - 1; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                NewArray[i, j] = OldArray[i, j];
            }
        }
        OldArray = NewArray;
        return;
    }
    else
    {
        var NewArray = new string[AccountCount - 1, 3];
        for (int i = 0,k=0; i < AccountCount; i++)
        {
            if (i == mode) continue;
            for (int j = 0; j < 3; j++)
            {                
                NewArray[k, j] = OldArray[i, j];                
            }
            k++;
        }
        OldArray = NewArray;
        return;
    }
}
bool HaveRole=false;
string[] AllInfo = File.ReadAllLines(path);
string[,]? FormedData = new string[AllInfo.Length,3];
int AccountCount = AllInfo.Length;
for (int i = 0; i < AllInfo.Length; i++)
    {
        string[] Temporary = AllInfo[i].Split(' ').ToArray();
        for (int j = 0; j < 3; j++)  FormedData[i, j] = Temporary[j];
    }


Console.WriteLine("Если вы зарегистрированы в базе нажмите 1,иначе 0");\

string[] playerInfo = File.ReadAllLines(playerPath);
List<Player> players = new List<Player>();
for (int i = 0,j=0; i < playerInfo.Length; i++)
{
    Player player = new Player();
    string[] Temporary = playerInfo[i].Split(' ').ToArray();

        player.name = Temporary[j];
        player.birth = Temporary[++j];
        player.matchCount = int.Parse(Temporary[++j]);
        player.goalCount = int.Parse(Temporary[++j]);
        player.assistCount = int.Parse(Temporary[++j]);
        player.penaltyMinutes = int.Parse(Temporary[++j]);
        players.Add(player);
        j = 0;
}
bool AlreadyRegistered=Convert.ToBoolean(Console.ReadLine());
if (AlreadyRegistered)
{
    bool AlreadyFind = false;
    while (!AlreadyFind)
    {
        Console.WriteLine("Введите логин: ");
        string? Login = Console.ReadLine();
        for (int i = 0; i < AllInfo.Length && !AlreadyFind && Login!=null; i++)
        {
            if (FormedData[i, 0] == Login)
            {
                Console.WriteLine("Логин найден в системе\n");
                while (true)
                {
                    Console.WriteLine("Введите пароль: ");
                    string? Passwd = Console.ReadLine();
                    if (VerifyHashedPassword(FormedData[i, 1], Passwd) && Passwd!=null)
                    {
                        Console.WriteLine("Вы вошли в систему");
                        AlreadyFind = true;
                        HaveRole = true;
                        break;
                    }
                    Console.WriteLine("Неправильный пароль,попробуйте снова");
                }
            }
        }
        if (!AlreadyFind) Console.WriteLine("В системе нет данного пользователя,попробуйте другой логин");
    }
}
else
{
    Console.WriteLine("Для регистрации введите логин");
    string? DataToFile;
    DataToFile =Console.ReadLine()+' ';
    Console.WriteLine("Введите пароль");
    DataToFile +=HashPassword(Console.ReadLine())+' '+0+ '\n';
    File.AppendAllText(path, DataToFile);
}
if (HaveRole)
{
    Console.WriteLine("Функции администрации\n1-Управление учётными записями\n2-Управление файлами\n3-Работа в данными");
    while (true)
    {        
        switch (Convert.ToInt32(Console.ReadLine()))
        {
            case 1:
                {
                    bool Exit=true;
                    while (Exit)
                    {
                        Console.WriteLine("1-Просмотр все учетных записей\n2-Добавить учётную запись\n3-Отредактировать учётную запись\n4-Удалить учётную запись\n5-Выйти обратно");
                        switch (Convert.ToInt32(Console.ReadLine()))
                        {
                            case 1:
                                {
                                    ShowInfo(FormedData, AccountCount);
                                    break;
                                }
                            case 2:
                                {
                                    bool Exit1 = true;
                                    while (Exit1)
                                    {                                        
                                        Console.WriteLine("1-Добавить учётную запись\n2-Выйти обратно\n");
                                        switch (Convert.ToInt32(Console.ReadLine()))
                                        {
                                            case 1:
                                                {
                                                    Console.WriteLine("Введите логин новой учетной записи");
                                                    string? Login = Console.ReadLine();
                                                    string? DataToFile = "";
                                                    Console.WriteLine("Введите пароль");
                                                    string? Passwd = Console.ReadLine();
                                                    DataToFile += Login + ' ' + HashPassword(Passwd) + ' ' + 0 + '\n';
                                                    File.AppendAllText(path, DataToFile);
                                                    AccountCount++;
                                                    ArrayResize(AccountCount, ref FormedData,-1);
                                                    FormedData[AccountCount-1, 0] = Login;
                                                    FormedData[AccountCount-1, 1] = HashPassword(Passwd);
                                                    FormedData[AccountCount-1, 2] = "0";
                                                    break;
                                                }
                                            case 2:
                                                {
                                                    Exit1 = false;
                                                    break;
                                                }
                                            default:
                                                Console.WriteLine("Выбирите пункт меню");
                                                break;
                                        }
                                    }
                                    break;
                                }
                            case 3:
                                {
                                    bool Exit1=true;
                                    while (Exit1)
                                    {
                                        ShowInfo(FormedData, AllInfo.Length);
                                        Console.WriteLine("Введите номер учётной записи для редактирования");
                                        int AccountNumber = Convert.ToInt32(Console.ReadLine());
                                        Console.WriteLine("1-Изменить логин\n2-Изменить пароль\n3-Изменить права\n4-вернуться обратно");
                                        switch (Convert.ToInt32(Console.ReadLine()))
                                        {
                                            case 1:
                                                {
                                                    Console.WriteLine("Введите новый логин: ");
                                                    FormedData[AccountNumber, 0] = Console.ReadLine();
                                                    break;
                                                }
                                            case 2:
                                                {
                                                    Console.WriteLine("Введите новый пароль: ");
                                                    FormedData[AccountNumber, 1] = HashPassword(Console.ReadLine());
                                                    break;                                                    
                                                }
                                            case 3:
                                                {
                                                    Console.WriteLine("Выбирите права 0-пользователь\n1-админ\n");
                                                    FormedData[AccountNumber, 2] = Console.ReadLine();
                                                    break;
                                                }
                                            case 4:
                                                {
                                                    Exit1 = false;
                                                    break;
                                                }
                                            default:
                                                Console.WriteLine("Выбирите пункт меню");
                                                break;
                                        }
                                        break;
                                    }
                                    break;
                                }
                            case 4:
                                {
                                    bool Exit1 = true;
                                    while (Exit1)
                                    {
                                        Console.WriteLine("1-Удаление учётной записи\n2-Выход обратно");
                                        switch (Convert.ToInt32(Console.ReadLine()))
                                        {
                                            case 1:
                                                {
                                                    ShowInfo(FormedData, AccountCount);
                                                    Console.WriteLine("Выбирите учётную запись для удаления");
                                                    int AccountDelete=Convert.ToInt32(Console.ReadLine());
                                                    ArrayResize(AccountCount,ref FormedData,AccountDelete-1);
                                                    AccountCount--;
                                                    string? DataToFile;
                                                    File.WriteAllText(path, DataToFile = FormedData[0, 0] + ' ' + FormedData[0, 1] + ' ' + FormedData[0, 2] + '\n');
                                                    for (int i = 1; i < AccountCount; i++)
                                                    {
                                                        DataToFile = FormedData[i, 0] + ' ' + FormedData[i, 1] + ' ' + FormedData[i, 2] + '\n';
                                                        File.AppendAllText(path, DataToFile);
                                                    }
                                                    break;
                                                }
                                            case 2:
                                                {
                                                    Exit1 = false;
                                                    break;
                                                }
                                            default:
                                                break;
                                        }
                                    }
                                    break;
                                }
                            case 5:
                                {
                                    Exit = false;
                                    break;
                                }
                            default:
                                break;
                        }
                    }                    
                    break;
                };
            case 2:
                {
                    Console.WriteLine("1-Создать файл\n2-Открыть файл\n3-Удалить файл");
                    switch (int.Parse(Console.ReadLine()))
                    {
                        case 1:
                            {
                                Console.WriteLine("Введите название файл");
                                File.Create(@$"E:\Курсач\Курсач\{Console.ReadLine()}.txt");
                                break;
                            }
                        case 2:
                            {
                                Console.WriteLine("Введите имя файла:");
                                File.Open(@$"E:\Курсач\Курсач\{Console.ReadLine()}.txt", FileMode.Open);
                                break;
                            }
                        case 3:
                            {
                                Console.WriteLine("Введите имя файла:");
                                File.Delete(@$"E:\Курсач\Курсач\{Console.ReadLine()}.txt");
                                break;
                            }
                        default:
                            Console.WriteLine("Неправильное число");
                            break;
                    }
                    break;
                };
                case 3:
                {
                    bool Exit=true;
                    while(Exit)
                    { 
                    Console.WriteLine("1 -Посмотреть все данные\n2-Добавить запись\n3-Удалить запись\n4-Редактировать запись\n5-Выполнить задачу\n6-Поиск данных\n7-Сортировка по кол-ву матчей\n8-Выход");
                        switch (int.Parse(Console.ReadLine()))
                        {
                            case 1:
                                {
                                    ShowListInfo(players);
                                    break;
                                }
                            case 2:
                                {
                                    Player player = new Player();
                                    Console.WriteLine("Введите имя");
                                    player.name = Console.ReadLine();
                                    Console.WriteLine("Введите дату рождения");
                                    player.birth = Console.ReadLine();
                                    Console.WriteLine("Введите кол-во матчей");
                                    player.matchCount = int.Parse(Console.ReadLine());
                                    Console.WriteLine("Введите кол-во голов");
                                    player.goalCount = int.Parse(Console.ReadLine());
                                    Console.WriteLine("Введите кол-во штрафных минут");
                                    player.penaltyMinutes = int.Parse(Console.ReadLine());
                                    Console.WriteLine("Введите кол-во передач");
                                    player.assistCount = int.Parse(Console.ReadLine());
                                    players.Add(player);
                                    break;
                                }
                            case 3:
                                {
                                    Console.WriteLine("Введите номер игрока для удаления");                                    
                                    ShowListInfo(players);
                                    players.Remove(players[int.Parse(Console.ReadLine())]);
                                    break;
                                }
                            case 4:
                                {
                                    bool Exit1 = true;
                                    Console.WriteLine("Введите номер игрока для редактирования");
                                    ShowListInfo(players);
                                    int playerNumber = int.Parse(Console.ReadLine());
                                    while(Exit1)
                                    {
                                        Console.WriteLine("1-ФИО\n2-Кол-во игр\n3-Кол-во голов\n4-Кол-во голевых передач\n5-Кол-во штрафных минут\n6-Дата рождения\n7-Выход");
                                        switch (int.Parse(Console.ReadLine()))
                                        {
                                            case 1:
                                                {
                                                    Console.WriteLine("Введите имя");
                                                    players[playerNumber].name = Console.ReadLine();
                                                    break;
                                                }
                                            case 2:
                                                {
                                                    Console.WriteLine("Введите кол-во матчей");
                                                    players[playerNumber].matchCount = int.Parse(Console.ReadLine());
                                                    break;
                                                }
                                            case 3:
                                                {
                                                    Console.WriteLine("Введите голов");
                                                    players[playerNumber].goalCount = int.Parse(Console.ReadLine());
                                                    break;
                                                }
                                            case 4:
                                                {
                                                    Console.WriteLine("Введите передач");
                                                    players[playerNumber].assistCount = int.Parse(Console.ReadLine());
                                                    break;
                                                }
                                            case 5:
                                                {
                                                    Console.WriteLine("Введите кол-во штрафных минут");
                                                    players[playerNumber].penaltyMinutes = int.Parse(Console.ReadLine());
                                                    break;
                                                }
                                            case 6:
                                                {
                                                    Console.WriteLine("Введите дату рождения");
                                                    players[playerNumber].birth = Console.ReadLine();
                                                    break;
                                                }
                                            case 7:
                                                {
                                                    Exit1=false;
                                                    break;
                                                }
                                            default:
                                                Console.WriteLine("Неправильная цифра");
                                                break;
                                        }
                                        
                                    }
                                    break;

                                }
                            case 5:
                                {
                                    var sortedPlayers = players.OrderBy(players => players.goalCount+players.assistCount);
                                    foreach (var player in sortedPlayers)
                                    {
                                        Console.WriteLine($"ФИО {player.name}\nДата рождения {player.birth}\nКол-во матчей {player.goalCount}\nКол-во передач {player.assistCount}\n");
                                        Console.WriteLine($"Кол-во штрафных минут {player.penaltyMinutes}\nКол-во голов {player.goalCount}\n");
                                        break;
                                    }
                                    break;
                                }
                            case 6:
                                {
                                    Console.WriteLine($"Введите кол-во игр");
                                    int playerCount=int.Parse(Console.ReadLine());
                                    foreach (var player in players)
                                    {
                                        if (playerCount==player.matchCount)
                                        {
                                            Console.WriteLine($"ФИО {player.name}\nДата рождения {player.birth}\nКол-во матчей {player.goalCount}\nКол-во передач {player.assistCount}\n");
                                            Console.WriteLine($"Кол-во штрафных минут {player.penaltyMinutes}\nКол-во голов {player.goalCount}\n");
                                            break;
                                        }
                                    }
                                    break;
                                }
                            case 7:
                                {
                                    var sortedPlayers = players.OrderByDescending(players => players.goalCount);
                                    foreach (var player in sortedPlayers)
                                    {
                                        Console.WriteLine($"ФИО {player.name}\nДата рождения {player.birth}\nКол-во матчей {player.goalCount}\nКол-во передач {player.assistCount}\n");
                                        Console.WriteLine($"Кол-во штрафных минут {player.penaltyMinutes}\nКол-во голов {player.goalCount}\n");
                                    }
                                    break;
                                }
                            case 8:
                                {
                                    Exit = false;
                                    break;
                                }
                            default:
                                break;
                        }
                    }
                    break;
                }
            default:
                Console.WriteLine("Введите подходящее число");
                break;
        }
    }    
}
else
{
    bool Exit2 = true;
    while (Exit2)
    {
        Console.WriteLine("1-Вывести всех\n2-Выполнить задачу\n3-Сортировка по кол-ву матчей\n4-Поиск данных\n5-Выход");
        switch (int.Parse(Console.ReadLine()))
        {
            case 1:
                {
                    ShowListInfo(players);
                    break;
                }
            case 2:
                {
                    var sortedPlayers = players.OrderByDescending(players => players.goalCount + players.assistCount);
                    foreach (var player in sortedPlayers)
                    {
                        Console.WriteLine($"ФИО {player.name}\nДата рождения {player.birth}\nКол-во матчей {player.goalCount}\nКол-во передач {player.assistCount}\n");
                        Console.WriteLine($"Кол-во штрафных минут {player.penaltyMinutes}\nКол-во голов {player.goalCount}\n");
                        break;
                    }
                    break;
                }
            case 3:
                {
                    var sortedPlayers = players.OrderByDescending(players => players.goalCount);
                    foreach (var player in sortedPlayers)
                    {
                        Console.WriteLine($"ФИО {player.name}\nДата рождения {player.birth}\nКол-во матчей {player.goalCount}\nКол-во передач {player.assistCount}\n");
                        Console.WriteLine($"Кол-во штрафных минут {player.penaltyMinutes}\nКол-во голов {player.goalCount}\n");
                    }
                    break;
                }
            case 4:
                {
                    Console.WriteLine($"Введите кол-во игр");
                    int playerCount = int.Parse(Console.ReadLine());
                
                    foreach (var player in players)
                    {
                        if (playerCount == player.matchCount)
                        {
                            Console.WriteLine($"ФИО {player.name}\nДата рождения {player.birth}\nКол-во матчей {player.goalCount}\nКол-во передач {player.assistCount}\n");
                            Console.WriteLine($"Кол-во штрафных минут {player.penaltyMinutes}\nКол-во голов {player.goalCount}\n");
                            break;
                        }
                    }
                    break;
                }
            case 5:
                {
                    Exit2 = false;
                    break;
                }
            default:
                Console.WriteLine($"Неправильное число");
                break;
        }
    }
    File.WriteAllText(playerPath, "");
    foreach (var player in players)
    {
        File.AppendAllText(playerPath, $"{player.name} {player.birth} {player.matchCount} {player.goalCount} {player.assistCount} {player.penaltyMinutes}\n");
    }
}
class Player
{
    public string? name;
    public string? birth;
    public int matchCount;
    public int goalCount;
    public int assistCount;
    public int penaltyMinutes;
};