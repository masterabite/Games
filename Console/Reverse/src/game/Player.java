package game;

import java.util.Random;
import java.util.Scanner;

public class Player {
    Game game;
    int id;                                 //номер игрока в игре
    String name;                            //имя игрока
    private char moveChar;                  //символ для хода игрока
    private Board board;                    //игровая доска
    private boolean bot;                    //является ли игрок ботом
    private int score;                      //общий счет игрока
    private int[] move;                     //ход игрока на текущий момент

    public Player(Game _game, int _id, String _name, char _moveChar, Board _board, boolean _bot) {
        game = _game;
        id = _id;
        moveChar = _moveChar;
        board = _board;
        bot = _bot;
        name = _name;
        score = 0;
        move = new int[2]; //ход задается массивом из двух чисел соответственно позиция на доске
    }

    //геттеры
    public String getName() {
        return name;
    }
    public char getMoveChar() {
        return moveChar;
    }
    public boolean isPlayer() {return !bot;}
    public int getScore() {
        return score;
    }
    public int[] getMove() {
        return move;
    }

    //функция сбрасывает счет игрока
    public void resetScore() {
        score = 2;
    }

    //игрок делает ход (выбирает клетку, в которую он будет ходить)
    public void makeMove() {
        if (bot) {
            try {
                Thread.sleep(1500);
            } catch (InterruptedException ignored) {
            }

            Player otherPlayer = game.getPlayers().get((id+1)%game.getPlayers().size());
            String AILevel = game.findParameter("сложность").getValue();

            int maxSum = 0;

            if (AILevel.equals("легко")) {
                for (int i = 0; i < board.getSize(); ++i) {
                    for (int j = 0; j < board.getSize(); ++j) {
                        int[] cell = new int[] {i, j};
                        int cellSum = 0;
                        for (int direction = 0; direction < board.getDirectionsCount(); ++direction) {
                            cellSum += Math.max(0, board.closedCellCount(this, otherPlayer, cell, direction));
                        }

                        boolean isBetter = false;
                        if (cellSum> maxSum) {
                            isBetter = true;
                        } else if (cellSum > 0 && cellSum == maxSum) {
                            Random random = new Random();
                            if (random.nextInt()%100 < 20) {
                                isBetter = true;
                            }
                        }

                        if (isBetter) {
                            maxSum = cellSum;
                            move = cell;
                        }
                    }
                }
            }

            if (move != null) {
                System.out.printf("%s выбирает клетку %d%c\n", name, 1 + move[0], 'A' + move[1]);
            }
        } else {
            Scanner scanner = new Scanner(System.in);
            while (true) {
                try {
                    System.out.println("Для выхода в меню введите 0");
                    System.out.print("Введите число и букву через пробел: ");
                    move[0] = Integer.parseInt(scanner.next()) - 1;

                    if (move[0] == -1) {
                        return;
                    }

                    move[1] = scanner.next().toUpperCase().charAt(0) - 'A';
                } catch (Exception e) {
                    scanner.reset();
                    continue;
                }
                break;
            }
            System.out.printf("%s выбирает клетку %d%c\n", name, 1+move[0], 'A'+move[1]);
        }
    }

    //функция увеличивает счёт игрока на 1
    public void incScore() {
        ++score;
    }

    //функция уменьшает счёт игрока на 1
    public void decScore() {
        --score;
    }
}
