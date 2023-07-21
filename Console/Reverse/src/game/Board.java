package game;

public class Board {
    private int size;                   //размер доски
    private char[][] field;             //символьное отображение доски
    private char defaultChar;           //символ, обозначающий пустую клетку

    //массив приращений для i и j в соответствующих девяти направлениях
    private static int[][] dij = {
            {0, 1,  1,  1,  0, -1, -1, -1},
            {1, 1,  0, -1, -1, -1,  0,  1}
    };

    public Board(int _size) {
        size = _size;
        field = new char[size][size];
        defaultChar = '.';
        fieldClear();
    }

    //функции доступа
    public int getSize() {
        return size;
    }
    public char getCharAt(int i, int j) {
        return field[i][j];
    }
    public void setCellChar(int[] cell, char c) {
        field[cell[0]][cell[1]] = c;
    }

    //функция возвращает кол-во направлений
    public int getDirectionsCount() {
        return dij[0].length;
    }

    //функция возвращает следующую клетку от cell в направлении direction на расстояние length
    public int[] nextCell(int[] cell, int direction, int length) {
        return new int[] {cell[0] + dij[0][direction]*length, cell[1] + dij[1][direction]*length};
    }

    //функция вычисляет кол-во закрываемых клеток игрока противника
    public int closedCellCount(Player player, Player otherPlayer, int[] cell, int direction) {
        if (cellFree(cell)) {

            //ищем ближайшую клетку не принадлежащую противнику в этом направлении
            int k = 1;
            while (cellBelong(nextCell(cell, direction, k), otherPlayer)) {
                ++k;
            }

            //если она принадлежит текущему игроку, значит он закрывает последовательность противника
            if (cellBelong(nextCell(cell, direction, k), player)) {
                return k - 1;     //длина последовательности противника
            }
        }

        return -1;
    }

    //функция проверяет существование клетки
    public boolean cellExist(int[] cell) {
        return cell != null && (cell[0] >= 0 && cell[1] >= 0 && cell[0] < size && cell[1] < size);
    }

    //функция приводит поле в начальное состояние
    public void fieldClear() {
        for (int i = 0; i < size; ++i) {
            for (int j = 0; j < size; ++j) {
                field[i][j] = defaultChar;
            }
        }
    }

    //функция проверяет, принадлежит ли клетка cell игроку player
    public boolean cellBelong(int[] cell, Player player) {
        return cellExist(cell) && field[cell[0]][cell[1]] == player.getMoveChar();
    }

    //функция проверяет, свободна ли клетка cell
    public boolean cellFree(int[] cell) {
        return field[cell[0]][cell[1]] == defaultChar;
    }
}
