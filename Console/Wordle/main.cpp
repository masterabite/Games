#define _CRT_SECURE_NO_WARNINGS
#include <iostream>
#include <time.h>

using namespace std;

#define max_n 10000  //максимальный размер слов
#define height 6    //высота поля (кол-во попыток)
#define width 5     //ширина слов

//обозначения цветов
#define RESET "\033[0m"
#define GRAY "\033[2m"
#define WHITE "\033[1m"
#define GREEN "\033[32m"
#define YELLOW "\033[33m"
#define RED "\033[31m"


char book[max_n][width + 1];        //словарь, хранит все слова
char field[height][width + 1];      //игровое поле
int word_index;                     //индекс загаданного слова в словаре


//функция очищает игровое поле
void field_clear() {
    for (int i = 0; i < height; ++i) {
        for (int j = 0; j < width; ++j) {
            field[i][j] = ' ';
        }
    }
}

//функция сравнения двух слов
int is_equal(const char w1[width], const char w2[width]) {
    for (int i = 0; i < width; ++i) {
        if (w1[i] != w2[i]) {
            return 0;
        }
    }
    return 1;
}

//функция которая ищет слово word в словаре\
и возвращает его индекс, или -1 если слово
int find_word(const char word[width]) {
    for (int i = 0; i < max_n; ++i) {
        if (is_equal(book[i], word)) {
            return i;
        }
    }
    return -1;
}

//функция добавляет слово, с индексом ind в словаре, в игровое поле\
возвращает: какая по счету попытка была потрачена
int field_add(const char word[width]) {
    int ind = find_word(word);

    if (ind == -1) {
        return -1;
    }

    for (int i = 0; i < height; ++i) {
        if (is_equal(field[i], word)) {
            return 0;
        }

        if (field[i][0] == ' ') {
            for (int j = 0; j < width; ++j) {
                field[i][j] = word[j];
            }
            return i + 1;
        }
    }
    return height;
}

//функция выводит [i][j]-ый символ поля в соответствующем цвете
void field_char_print(int i, int j) {

    //если буква на своем месте, сразу выводим зеленым
    if (field[i][j] == book[word_index][j]) {
        printf("%s%c%s", GREEN, field[i][j], RESET);
        return;
    }

    //рассчитываем, есть ли соответствующая буква, на несоответствующем месте
    int cnt = 0;
    for (int q = 0; q < width; ++q) {
        if (book[word_index][q] == field[i][j] && book[word_index][q] != field[i][q]) {
            ++cnt;
        }
    }
    for (int q = 0; q <= j; ++q) {
        if (field[i][j] == field[i][q]) {
            --cnt;
        }
    }

    //вывод буквы
    if (cnt >= 0) {
        printf("%s%c%s", YELLOW, field[i][j], RESET);
    }
    else {
        printf("%s%c%s", GRAY, field[i][j], RESET);
    }

}

//функция выводит символ в цвете (для клавишей)
void char_print(char c) {
    int col = 0;

    if (c >= 'A' || c <= 'Z') {

        for (int j = 0; j < width; ++j) {
            for (int i = 0; i < height; ++i) {
                if (c == field[i][j]) {
                    col = 1;
                    if (c == book[word_index][j]) {
                        printf("%s%c%s", GREEN, c, RESET);
                        return;
                    }
                }
            }
        }


        for (int j = 0; j < width; ++j) {
            for (int i = 0; i < height; ++i) {
                if (c == field[i][j]) {
                    for (int k = 0; k < width; ++k) {
                        if (c == book[word_index][k]) {
                            printf("%s%c%s", YELLOW, c, RESET);
                            return;
                        }
                    }
                }
            }
        }

        if (col == 1) {
            printf(" ");
        }
        else {
            printf("%s%c%s", WHITE, c, RESET);
        }
        return;
    }
    printf("%s%c", RESET, c);
}


//функция выводит игровое в консоль
void field_print() {
    const char* str = "\n+---+---+---+---+---+\n";

    for (int i = 0; i < height; ++i) {
        printf("%s|", str);
        for (int j = 0; j < width; ++j) {
            printf(" ");
            field_char_print(i, j);
            printf(" |");
        }
    }printf("%s", str);

    const char* alp = "\n Q W E R T Y U I O P\n  A S D F G H J K L\n   Z X C V B N M \n";

    for (int i = 0; alp[i] != '\0'; ++i) {
        char_print(alp[i]);
    }
}


int main() {
    srand(time(NULL));

    /*
     * file reading
     * */
    string fname = "book.txt";
    FILE* fin = fopen(fname.c_str(), "r");
    int game_result = 0;

    if (fin == NULL) {
        //FILE* fin = fopen(fname, "w");
        printf("File \"%s\" not found...\n", fname.c_str());
        return 0;
    }

    int n = 0;
    while (!feof(fin)) {
        fscanf(fin, "%s", book[n++]);

        for (int i = 0; i < width; ++i) {
            if (book[n - 1][i] >= 'a' && book[n - 1][i] <= 'z') {
                book[n - 1][i] += 'A' - 'a';
            }
        }
    }
    fclose(fin);


    /*
     *Игровой процесс
     * */
    word_index = rand() % n;

    //очищаем игровое поле и сразу выводим
    field_clear();
    field_print();

    //основной цикл
    while (1) {
        char buffer[max_n]; //буффер ввода
        int add_result; //результать добавления слова в игровое пространство

        //цикл для ввода слова
        while (1) {
            printf("\nEnter a word: ");

            //считываем слово и делаем все строчные буквы заглавными
            scanf("%s", buffer);
            for (int j = 0; j < width; ++j) {
                if (buffer[j] >= 'a' && buffer[j] <= 'z') {
                    buffer[j] += 'A' - 'a';
                }
            }

            //пытаемся добавить слово
            add_result = field_add(buffer);

            if (is_equal(buffer, book[word_index])) {
                //случай, если ввели загаданное слово
                game_result = 1;
                break;
            }
            else if (add_result == height) {
                //если не отгадали и ввели последнее слово
                game_result = -1;
            }

            if (add_result == -1) { //ввели не найденное в словаре
                printf("there is no such word!\n");
            }
            else if (add_result == 0) { //ввели уже используемое слово
                printf("this word has already been!\n");
            }
            else {
                break;
            }
        }

        system("cls");

        //выводим информацию
        field_print();

        //если игра завершена выходим из цикла
        if (game_result == 1 || add_result == height) {
            break;
        }
    }printf("\n");


    //выводим сообщение, соответствующее результату игры
    if (game_result == 1) {
        printf("%sYou gave the word! %s", GREEN, WHITE);
    }
    else if (game_result == -1) {
        printf("%sYou used up all the attempts \nand did not guess the word: %s", RED, WHITE);
    }

    //выводим загаданное слово
    for (int j = 0; j < width; ++j) {
        printf("%c", book[word_index][j]);
    }printf("%s", RESET);

    return 0;
}
