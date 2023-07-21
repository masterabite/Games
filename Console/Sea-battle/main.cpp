#include <iostream>
#include <time.h>

using namespace std;

//размер поля
const int n = 5;

//длина максимального корабля
const int m = 2;

//массив для четырех направлений движений
const int dxy[2][4] = { {1, 0, -1, 0}, {0, 1, 0, -1} };

//массив соседних клеток
const int dxy8[2][8] = { {1, 0, -1, 0, 1, -1, 1, -1}, {0, 1, 0, -1, 1, -1, -1, 1} };

//Символы поля
const char shipChar = '#';
const char missChar = '.';
const char destroyChar = 'x';
const char emptyChar = ' ';


void setRandomPosition(int& x, int& y) {
    x = (rand() % n);
    y = (rand() % n);
}

//структура игрока
struct Player {

    char field[n][n]; //поле в виде матрицы символов
    int shipCount; //количество целых кораблей

    //конструктор по умолчанию
    Player() {
        fieldClear();
        shipCount = 0;
    }

    //функция очистки поля
    void fieldClear() {
        for (int i = 0; i < n; ++i) {
            for (int j = 0; j < n; ++j) {
                field[i][j] = emptyChar;
            }
        }
    }

    //функция выводит в консоль поле игрока
    void printField(string str) {
        cout << str << '\n';

        cout << "   ";
        for (int i = 0; i < n; ++i) {
            cout << char('A' + i) << ' ';
        }cout << '\n';

        for (int i = 0; i < n; ++i) {
            cout << (i < 9? " " : "") << i + 1 << ' ';

            for (int j = 0; j < n; ++j) {
                cout << field[i][j] << ' ';
            }cout << '\n';
        }
    }

    //функция которая проверяет, попадает ли клетка в поле
    bool inField(int x, int y) {
        return (x >= 0 && x < n&& y >= 0 && y < n);
    }

    //функция пытается поместить корабль длинной length
    //в клетку [x,y] по направлению direction
    bool createShip(int x, int y, int length, int direction) {

        //находим координаты крайней точки корабля.
        int lx = x + dxy[0][direction] * (length - 1);
        int ly = y + dxy[1][direction] * (length - 1);

        //проверяем, что корабль не выходит за пределы доски
        for (int i = min(x, lx); i <= max(x, lx); ++i) {
            for (int j = min(y, ly); j <= max(y, ly); ++j) {
                if (!inField(i, j)) {
                    return false;
                }
            }
        }

        //проверяем, что рядом нет кораблей
        for (int i = min(x, lx) - 1; i <= max(x, lx) + 1; ++i) {
            for (int j = min(y, ly) - 1; j <= max(y, ly) + 1; ++j) {
                if (inField(i, j) && field[i][j] == shipChar) {
                    return false;
                }
            }
        }



        //если всё в порядке, ставим корабль
        for (int i = min(x, lx); i <= max(x, lx); ++i) {
            for (int j = min(y, ly); j <= max(y, ly); ++j) {
                field[i][j] = shipChar;
            }
        }

        ++shipCount;
        return true;
    }

    //функция расставления кораблей случайным образом
    void fieldFill() {
        int x = -1, y = -1; //промежуточные координаты которые мы будем использовать
        int dir = rand() % 4;

        for (int i = m; i > 0; --i) {
            for (int j = i; j <= m; ++j) {

                while (!createShip(x, y, i, dir)) {
                    setRandomPosition(x, y);
                    dir = rand() % 4;
                }
            }
        }
    }

    //рекурсивная функция проверяющая, убит ли корабль, находящийся в точке x y
    bool checkShip(int x, int y) {
        for (int i = 0; i < 4; ++i) {
            int len = 0;
            while (inField(x + dxy[0][i] * len, y + dxy[1][i] * len) &&
                field[x + dxy[0][i] * len][y + dxy[1][i] * len] == destroyChar) {
                ++len;
            }

            if (inField(x + dxy[0][i] * len, y + dxy[1][i] * len) &&
                field[x + dxy[0][i] * len][y + dxy[1][i] * len] == shipChar) {
                return true;
            }
        }

        return false;
    }

    //функция обводит разрушенный корабль
    void circleShip(int x, int y, bool check[n][n]) {
        check[x][y] = true;
        for (int i = 0; i < 8; ++i) {
            if (inField(x + dxy8[0][i], y + dxy8[1][i])) {
                if (field[x + dxy8[0][i]][y + dxy8[1][i]] != destroyChar) {
                    field[x + dxy8[0][i]][y + dxy8[1][i]] = missChar;
                }
                else if (!check[x + dxy8[0][i]][y + dxy8[1][i]]) {
                    circleShip(x + dxy8[0][i], y + dxy8[1][i], check);
                }
            }
        }
    }


    /*
        функция атаки клетки field[x][y]
        -1  Вне поля
         0  Мимо
         1  Попал
         2  Убил
    */
    int attack(int x, int y) {
        if (!inField(x, y) || 
            field[x][y] == missChar || 
            field[x][y] == destroyChar) 
        {
            return -1;
        }

        if (field[x][y] == shipChar) {
            field[x][y] = destroyChar;


            if (!checkShip(x, y)) {

                bool check[n][n];
                for (int i = 0; i < n; ++i) {
                    for (int j = 0; j < n; ++j) {
                        check[i][j] = false;
                    }
                }

                circleShip(x, y, check);
                --shipCount;
                
                return 2; 
            }
            return 1;
        }

        if (field[x][y] == emptyChar) {
            field[x][y] = missChar;
        }

        return 0;
    }
};

void printFields(Player& p1, Player& p2) {
    cout << "\nВаше поле: " << '\n';

    cout << "   ";
    for (int i = 0; i < n; ++i) {
        cout << char('A' + i) << ' ';
    }
    
    cout << "\t\t";

    cout << "   ";
    for (int i = 0; i < n; ++i) {
        cout << char('A' + i) << ' ';
    }

    cout << '\n';

    for (int i = 0; i < n; ++i) {
        cout << (i < 9? " " : "") << i + 1 << ' ';

        for (int j = 0; j < n; ++j) {
            cout << p1.field[i][j] << ' ';
        }

        cout << "\t\t";
        
        cout << (i < 9? " " : "") << i + 1 << ' ';
        for (int j = 0; j < n; ++j) {
            cout << p2.field[i][j] << ' ';
        }

        cout << '\n';
    }
}

int main() {
    setlocale(LC_ALL, "Russian");
    srand(time(NULL));

    Player p1, p2;
    p2.fieldFill();

    int cmd = -1;

    //заполнение поля
    cout << "0-случайно 1-вручную\n";
    while (cmd < 0 || cmd>1) {
        cout << "Введите тип заполнения вашего поля: ";
        cin >> cmd;
        if (cmd == 0) {
            p1.fieldFill();
        }
        else if (cmd == 1) {
            //ввод поля вручную
            for (int i = m; i > 0; --i) {
                for (int j = i; j <= m; ++j) {

                    int num, dir, sx, sy;
                    char let;

                    p1.printField("\nВаше поле на данный момент:");

                    cout << "\nКорабль длинной " << i << ":";

                    while (true) {
                        cout << "\nВведите клетку в формате \"1 A\": ";
                        cin >> num >> let;
                        sx = num - 1;
                        sy = int(let - 'A');

                        cout << "0-вниз 1-вправо 2-вверх 3-влево\n";
                        cout << "Введите направление: ";
                        cin >> dir;

                        if (dir >= 0 && dir < 4 && p1.createShip(sx, sy, i, dir)) {
                            break;
                        }
                        cout << "Не удалось поставить корабль...\nПовторите попытку\n";
                    }
                }
            }
        }
        else {
            cout << "Неверная команда.";
        }
    }


    cmd = -1;
    int m = rand() % 2;
    int num, dir, mx, my, res;
    char let;
    bool next;

    //Дополнительный игрок, чтобы частично скрывать поле противника
    Player enemy;
    
    printFields(p1, enemy);

    cout << "\nНачало игры...";

    while (true) {
        if (m == 0) {
            cout << "\nВаш ход...";
            next = false;
            
            cout << "\nВведите клетку в формате: \"1 A\": ";
            cin >> num >> let;
            mx = num - 1;
            my = int(let - 'A');

            res = p2.attack(mx, my);
            if (res == 1) {
                cout << "Попал!";
                enemy.field[mx][my] = destroyChar;
            }
            else if (res == 2) {
                cout << "Убил!";
                enemy.field[mx][my] = destroyChar;

                bool check[n][n];
                for (int i = 0; i < n; ++i) {
                    for (int j = 0; j < n; ++j) {
                        check[i][j] = false;
                    }
                }
                enemy.circleShip(mx, my, check);
            }
            else if (res == -1) {
                cout << "Выберите другую клетку!";
                continue;
            }
            else if (res == 0) {
                cout << "Мимо...";
                enemy.field[mx][my] = missChar;
                next = true;
            }
        }
        else {

            mx = -1;
            my = -1;

            while (!p1.inField(mx, my) ||
                p1.field[mx][my] == missChar ||
                p1.field[mx][my] == destroyChar)
            {
                mx = rand() % n;
                my = rand() % n;
            }

            cout << "\nХод соперника...";
            printf("\nОн выбирает клетку %d %c...", mx + 1, 'A' + my);
            res = p1.attack(mx, my);
            if (res == 1) {
                cout << "\nПопал!";
                printFields(p1, enemy);
                dir = rand() % 4;

                int newX = mx + dxy[0][dir];
                int newY = my + dxy[1][dir];

                //выбирает сторону
                while (!p1.inField(newX, newY) || p1.field[newX][newY] == destroyChar) {
                    dir = rand() % 4;
                    newX = mx + dxy[0][dir];
                    newY = my + dxy[1][dir];
                }
                printf("\nСоперник выбирает клетку %d %c...", newX + 1, 'A' + newY);
                res = p1.attack(newX, newY);
                while (p1.inField(newX, newY) && res == 1) {
                    cout << "\nПопал!";
                    printFields(p1, enemy);
                    newX = newX + dxy[0][dir];
                    newY = newY + dxy[1][dir];
                    printf("\nСоперник выбирает клетку %d %c...", newX + 1, 'A' + newY);
                }
            }

            if (res == 2) {
                cout << "\n Ваш корабль разрушен!";
            }

            if (res == 0) {
                cout << "\nМимо...";
                next = true;
            }
        }


        //ход переходит следующему игроку
        if (next) {
            ++m;
            m %= 2;
        }
        
        printFields(p1, enemy);
        cout << '\n';

        if (p1.shipCount == 0) {
            cout << "\nСоперник выйграл!";
            break;
        }

        if (p2.shipCount == 0) {
            cout << "\nВы выйграли!";
            break;
        }
    }


    return 0;
}
