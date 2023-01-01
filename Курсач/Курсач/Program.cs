//Разработка программы учета сведений об игроках хоккейной команды.
//	Сведения об игроках хоккейной команды включают: Ф.И.О.игрока, дату рождения, количество сыгранных матчей
//	число заброшенных шайб, количество голевых передач, количество штрафных минут.
//	Индивидуальное задание: вывести 6 лучших игроков(голы+передачи) с указанием их результативности.

using System.Security.Cryptography;

const string autorizationPath= @"E:\Курсач\Курсач\autorizationData.txt";
const string playerPath = @"E:\Курсач\Курсач\playersData.txt";
static void ShowSortedData(IOrderedEnumerable<Player> sortedPlayers)
{
    foreach (var player in sortedPlayers)
    {
        player.ShowPlayer();
        break;
    }
}
static void ShowAccountInfo(string[,] FormedData,int Count)
{
    for (int i = 0; i < Count; i++)
    {
        Console.WriteLine($"Логин:{FormedData[i, 0]}\nПароль:{FormedData[i, 1]}\nПрава:{FormedData[i, 2]}\n");
    }
}
static string HashPassword(string password)
{
    if (password == null) throw new ArgumentNullException("password");
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
static void ShowListInfo(List<Player> players)
{
    foreach (var player in players)
    {
        player.ShowPlayer();
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
static bool Authorization(string[,]? FormedData,int count)
{
    Console.WriteLine("Если вы зарегистрированы в базе нажмите 1");
    if (Console.ReadLine() == "1")
    {
        while (true)
        {
            Console.WriteLine("Введите логин: ");
            string? Login = Console.ReadLine();
            for (int i = 0; i < count &&  Login != null; i++)
            {
                if (FormedData[i, 0] == Login)
                {
                    Console.WriteLine("Логин найден в системе\n");
                    while (true)
                    {
                        Console.WriteLine("Введите пароль: ");
                        string? Passwd = Console.ReadLine();
                        if (VerifyHashedPassword(FormedData[i, 1], Passwd) && Passwd != null)
                        {
                            return true;
                        }
                        Console.WriteLine("Неправильный пароль,попробуйте снова");
                    }
                }
            }
            Console.WriteLine("В системе нет данного пользователя,попробуйте другой логин");
        }
    }
    return Registration(FormedData,count);
}
static bool Registration(string[,]? FormedData, int count)
{
    while (true)
    {
        bool AlreadyHave = true;
        string? DataToFile;
        Console.WriteLine("Для регистрации введите логин");
        DataToFile=Console.ReadLine();
        for (int i = 0; i < count; i++)
        {
            if (DataToFile == FormedData[i,0])
            {
                Console.WriteLine("Такой логин уже сущесвует в системе");
                AlreadyHave=false;
                break;
            }
        }
        if(AlreadyHave)
        {
            DataToFile += ' ';
            Console.WriteLine("Введите пароль");
            DataToFile += HashPassword(Console.ReadLine()) + ' ' + 0 + '\n';
            File.AppendAllText(autorizationPath, DataToFile);
            return false;
        }
    }
}
static void SetPlayerInfo(ref List<Player> players)
{
    Console.Clear();
    Player player = new Player();
    Console.WriteLine("Введите имя");
    player.name = Console.ReadLine();
    Console.WriteLine("Введите дату рождения");
    while (!DateOnly.TryParse(Console.ReadLine(), out player.birth)) ;
    Console.WriteLine("Введите кол-во матчей");
    player.MatchCount = 0;
    Console.WriteLine("Введите кол-во голов");
    player.GoalCount = 0;
    Console.WriteLine("Введите кол-во штрафных минут");
    player.PenaltyMinutes = 0;
    Console.WriteLine("Введите кол-во передач");
    player.AssistCount = 0;
    players.Add(player);
    Console.Clear();
}


string[] AllInfo = File.ReadAllLines(autorizationPath);
string[,]? FormedData = new string[AllInfo.Length,3];
int accountCount = AllInfo.Length;
for (int i = 0; i < AllInfo.Length; i++)
    {
        string[] Temporary = AllInfo[i].Split(' ').ToArray();
        for (int j = 0; j < 3; j++)  FormedData[i, j] = Temporary[j];
    }
string[] playerInfo = File.ReadAllLines(playerPath);
List<Player> players = new List<Player>();
for (int i = 0,j=0; i < playerInfo.Length; i++)
{
    Player player = new Player();
    string[] Temporary = playerInfo[i].Split(' ').ToArray();
        player.name = Temporary[j];
        player.birth = DateOnly.Parse(Temporary[++j]);
        player.matchCount = int.Parse(Temporary[++j]);
        player.goalCount = int.Parse(Temporary[++j]);
        player.assistCount = int.Parse(Temporary[++j]);
        player.penaltyMinutes = int.Parse(Temporary[++j]);
        players.Add(player);
        j = 0;
}

if (Authorization(FormedData,AllInfo.Length))
{
    bool Exitt = true;
    while (Exitt)
    {
        Console.Clear();
        Console.WriteLine("Функции администрации\n1-Управление учётными записями\n2-Управление файлами\n3-Работа в данными\n4-Выход из программы");
        switch (Convert.ToInt32(Console.ReadLine()))
        {
            case 1:
                {
                    bool Exit=true;
                    Console.Clear();
                    while (Exit)
                    {
                        Console.WriteLine("1-Просмотр все учетных записей\n2-Добавить учётную запись\n3-Отредактировать учётную запись\n4-Удалить учётную запись\n5-Выйти обратно");
                        switch (Convert.ToInt32(Console.ReadLine()))
                        {
                            case 1:
                                {
                                    Console.Clear();
                                    ShowAccountInfo(FormedData, accountCount);
                                    break;
                                }
                            case 2:
                                {
                                    Console.Clear();
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
                                                    DataToFile += Login + ' ' + Passwd.GetHashCode() + ' ' + 0 + '\n';
                                                    File.AppendAllText(autorizationPath, DataToFile);
                                                    accountCount++;
                                                    ArrayResize(accountCount, ref FormedData,-1);
                                                    FormedData[accountCount-1, 0] = Login;
                                                    FormedData[accountCount-1, 1] = HashPassword(Passwd);
                                                    FormedData[accountCount-1, 2] = "0";
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
                                    Console.Clear();
                                    bool Exit1 =true;
                                    while (Exit1)
                                    {
                                        ShowAccountInfo(FormedData, AllInfo.Length);
                                        Console.WriteLine("Введите номер учётной записи для редактирования");
                                        int AccountNumber = Convert.ToInt32(Console.ReadLine())-1;
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
                                    Console.Clear();
                                    bool Exit1 = true;
                                    while (Exit1)
                                    {
                                        Console.WriteLine("1-Удаление учётной записи\n2-Выход обратно");
                                        switch (Convert.ToInt32(Console.ReadLine()))
                                        {
                                            case 1:
                                                {
                                                    ShowAccountInfo(FormedData, accountCount);
                                                    Console.WriteLine("Выбирите учётную запись для удаления");
                                                    int AccountDelete=Convert.ToInt32(Console.ReadLine());
                                                    Console.WriteLine("Вы точно хотите удалить учетную запись?\n1-Да\n2-Нет");
                                                    if (Console.ReadLine() == "2") break;
                                                    ArrayResize(accountCount,ref FormedData,AccountDelete-1);
                                                    accountCount--;
                                                    string? DataToFile;
                                                    File.WriteAllText(autorizationPath, DataToFile = FormedData[0, 0] + ' ' + FormedData[0, 1] + ' ' + FormedData[0, 2] + '\n');
                                                    for (int i = 1; i < accountCount; i++)
                                                    {
                                                        DataToFile = FormedData[i, 0] + ' ' + FormedData[i, 1] + ' ' + FormedData[i, 2] + '\n';
                                                        File.AppendAllText(autorizationPath, DataToFile);
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
                                Console.WriteLine("Выбирите пункт меню");
                                break;
                        }
                    }                    
                    break;
                };
            case 2:
                {
                    Console.Clear();
                    Console.WriteLine("1-Создать файл\n2-Открыть файл\n3-Удалить файл");
                    switch (int.Parse(Console.ReadLine()))
                    {
                        case 1:
                            {
                                Console.Clear();
                                Console.WriteLine("Введите название файл");
                                File.Create(@$"E:\Курсач\Курсач\{Console.ReadLine()}.txt");
                                break;
                            }
                        case 2:
                            {
                                Console.Clear();
                                Console.WriteLine("Введите имя файла:");
                                File.Open(@$"E:\Курсач\Курсач\{Console.ReadLine()}.txt", FileMode.Open);
                                break;
                            }
                        case 3:
                            {
                                Console.Clear();
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
                    Console.Clear();
                    bool Exit =true;
                    while(Exit)
                    {
                        
                        Console.WriteLine("1-Посмотреть все данные\n2-Добавить запись\n3-Удалить запись\n4-Редактировать запись\n5-Выполнить задачу\n6-Поиск данных\n7-Сортировка по кол-ву матчей\n8-Выход");
                        switch (int.Parse(Console.ReadLine()))
                        {
                            case 1:
                                {
                                    Console.Clear();
                                    ShowListInfo(players);
                                    break;
                                }
                            case 2:
                                {
                                    Console.Clear();
                                    SetPlayerInfo(ref players);
                                    break;
                                }
                            case 3:
                                {
                                    Console.Clear();
                                    Console.WriteLine("Введите номер игрока для удаления");                                    
                                    ShowListInfo(players);
                                    players.Remove(players[int.Parse(Console.ReadLine())]);
                                    break;
                                }
                            case 4:
                                {
                                    bool Exit1 = true;
                                    Console.Clear();
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
                                                    players[playerNumber].MatchCount = 0;
                                                    break;
                                                }
                                            case 3:
                                                {
                                                    Console.WriteLine("Введите голов");
                                                    players[playerNumber].GoalCount = 0;
                                                    break;
                                                }
                                            case 4:
                                                {
                                                    Console.WriteLine("Введите передач");
                                                    players[playerNumber].AssistCount = 0;
                                                    break;
                                                }
                                            case 5:
                                                {
                                                    Console.WriteLine("Введите кол-во штрафных минут");
                                                    players[playerNumber].PenaltyMinutes = 0;
                                                    break;
                                                }
                                            case 6:
                                                {
                                                    Console.WriteLine("Введите дату рождения");
                                                    while(!DateOnly.TryParse(Console.ReadLine(),out players[playerNumber].birth));
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
                                    var sortedPlayers = players.OrderByDescending(players => players.goalCount+players.assistCount);
                                    int count = 0;
                                    foreach (var player in sortedPlayers)
                                    {
                                        player.ShowPlayer();
                                        count++;
                                        if (count == 6) break;
                                    }
                                    break;
                                }
                            case 6:
                                {
                                    bool Exit11 = true;
                                    while (Exit11)
                                    {
                                        Console.Clear();
                                        Console.WriteLine("1-Поиск по ФИО\n2-Поиск по кол-ву игр\n3-Поиск по кол-ву голов\n4-Поиск по помощей\n5-Кол-во штрафных минут\n6-Поиск по дате");
                                        switch (int.Parse(Console.ReadLine()))
                                        {
                                            case 1:
                                                {
                                                    Console.Clear();
                                                    Console.WriteLine($"Введите ФИО");
                                                    string playerName = Console.ReadLine();
                                                    foreach (var player in players)
                                                    {
                                                        if (playerName == player.name)
                                                        {
                                                            player.ShowPlayer();
                                                            break;
                                                        }
                                                    }
                                                    break;
                                                }
                                            case 2:
                                                {
                                                    Console.Clear();
                                                    Console.WriteLine($"Введите кол-во игр");
                                                    int playerCount = int.Parse(Console.ReadLine());
                                                    foreach (var player in players)
                                                    {
                                                        if (playerCount == player.matchCount)
                                                        {
                                                            player.ShowPlayer();
                                                            break;
                                                        }
                                                    }
                                                    break;
                                                }
                                            case 3:
                                                {
                                                    Console.Clear();
                                                    Console.WriteLine($"Введите кол-во голов");
                                                    int playerGoal = int.Parse(Console.ReadLine());
                                                    foreach (var player in players)
                                                    {
                                                        if (playerGoal == player.goalCount)
                                                        {
                                                            player.ShowPlayer();
                                                            break;
                                                        }
                                                    }
                                                    break;
                                                }
                                            case 4:
                                                {
                                                    Console.Clear();
                                                    Console.WriteLine($"Введите кол-во помощей");
                                                    int playerAssist = int.Parse(Console.ReadLine());
                                                    foreach (var player in players)
                                                    {
                                                        if (playerAssist == player.assistCount)
                                                        {
                                                            player.ShowPlayer();
                                                            break;
                                                        }
                                                    }
                                                    break;
                                                }
                                            case 5:
                                                {
                                                    Console.Clear();
                                                    Console.WriteLine($"Введите кол-во игр");
                                                    int playerCount = int.Parse(Console.ReadLine());
                                                    foreach (var player in players)
                                                    {
                                                        if (playerCount == player.matchCount)
                                                        {
                                                            player.ShowPlayer();
                                                            break;
                                                        }
                                                    }
                                                    break;
                                                }
                                            case 6:
                                                {
                                                    Console.Clear();
                                                    Console.WriteLine($"Введите дату");
                                                    DateOnly playerCount = DateOnly.Parse(Console.ReadLine());
                                                    foreach (var player in players)
                                                    {
                                                        if (playerCount == player.birth)
                                                        {
                                                            player.ShowPlayer();
                                                            break;
                                                        }
                                                    }
                                                    break;
                                                }
                                            default:
                                                break;
                                        }
                                    }
                                    break;
                                }
                            case 7:
                                {
                                    var sortedPlayers = players.OrderByDescending(players => players.goalCount);
                                    ShowSortedData(sortedPlayers);
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
            case 4:
                {
                    Exitt = false;
                    break;
                }
            default:
                Console.WriteLine("Введите подходящее число");
                break;
        }
    }
    File.WriteAllText(playerPath, "");
    foreach (var player in players)
    {
        File.AppendAllText(playerPath, $"{player.name} {player.birth} {player.matchCount} {player.goalCount} {player.assistCount} {player.penaltyMinutes}\n");
    }
}
else
{
    bool Exit2 = true;
    while (Exit2)
    {
        Console.Clear();
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
                    int count = 0;
                    foreach(var player in sortedPlayers)
                    {
                        player.ShowPlayer();
                        count++;
                        if (count == 6) break;
                    }
                    break;
                }
            case 3:
                {
                    var sortedPlayers = players.OrderByDescending(players => players.goalCount);
                    ShowSortedData(sortedPlayers);
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
                            player.ShowPlayer();
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
    
}
class Player
{
    public string? name="Неизвестно";
    public DateOnly birth = new DateOnly();
    public int matchCount=0;
    public int MatchCount {
        
       set
        {
            while (true)
            {
                value = int.Parse(Console.ReadLine());
                if (value < 0)
                    Console.WriteLine("Кол-во игр должно быть не меньше 0");
                else
                {
                    matchCount = value;
                    break;
                }
            }
        }
        get
        {
            return matchCount;
        }
    }
    public int goalCount=0;
    public int GoalCount
    {

        set
        {
            while (true)
            {
                value = int.Parse(Console.ReadLine());
                if (value < 0)
                    Console.WriteLine("Кол-во голов должно быть не меньше 0");
                else
                {
                    goalCount = value;
                    break;
                }
            }
        }
        get
        {
            return goalCount;
        }
    }
    public int assistCount = 0;
    public int AssistCount
    {
        set
        {
            while (true)
            {
                value = int.Parse(Console.ReadLine());
                if (value < 0)
                    Console.WriteLine("Кол-во передач должно быть не меньше 0");
                else { 
                assistCount = value;
                break;
            }
        }
        }
        get
        {
            return assistCount;
        }
    }
    public int penaltyMinutes = 0;
    public int PenaltyMinutes
    {
        set
        {
            while (true)
            {
                value = int.Parse(Console.ReadLine());
                if (value < 1)
                    Console.WriteLine("Кол-во штрафных минут должно быть не меньше 0");
                else
                {
                    penaltyMinutes = value;
                    break;
                }
            }
        }
        get
        {
            return penaltyMinutes;
        }
    }
    public void ShowPlayer()
    {
        Console.WriteLine($"==================================\nФИО {name}\nДата рождения {birth}\nКол-во матчей {goalCount}\nКол-во передач {assistCount}");
        Console.WriteLine($"Кол-во штрафных минут {penaltyMinutes}\nКол-во голов {goalCount}\n==================================\n");
    }
    
};