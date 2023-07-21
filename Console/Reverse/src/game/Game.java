package game;

import exceptions.BoardOutException;
import exceptions.PlayerCantSelectCellException;
import menu.Room;
import menu.parameters.*;

import java.io.*;
import java.util.ArrayList;

public class Game {
    private ArrayList<Player> players;      //список всех игроков
    private int currentPlayerIndex;         //индекс игрока, который ходит
    private Board board;                    //игровая доска
    private ArrayList<Room> menu;           //меню игры в виде списка комнат

    public Game() {
        board = new Board(8);

        players = new ArrayList<>();

        menu = new ArrayList<>();

        menu.add(new Room("Главное меню", "Выход;Играть;Настройки;Рекорды"));

        Room settingsRoom = new Room("Настройки", "Назад");
        settingsRoom.addParameter(new ListParameter("режим",
                "игрок против ИИ;игрок против игрока;ИИ против ИИ".split(";"))
        );
        settingsRoom.addParameter(new ListParameter("сложность", "легко".split(";")));
        settingsRoom.addParameter(new SwitchParameter("подсказки", true));
        settingsRoom.addParameter(new StringParameter("ник1", "player"));
        settingsRoom.addParameter(new StringParameter("ник2", "bot"));
        menu.add(settingsRoom);

        Room recordsRoom = new Room("Рекорды", "Назад");
        recordsRoom.addParameter(new InformationParameter("лучший игрок", "- 0"));
        menu.add(recordsRoom);

        loadSettings();
    }

    public ArrayList<Player> getPlayers() {
        return players;
    }

    //функция ищет параметр игры по названию среди всех комнат
    public Parameter findParameter(String parameterName) {
        for (Room currentRoom : menu) {
            for (Parameter parameter : currentRoom.getParameters()) {
                if (parameter.getName().equals(parameterName)) {
                    return parameter;
                }
            }
        }
        return null;
    }

    //функция выводит доску в соответствие с параметром подсказки для игрока
    public void printBoard() {
        System.out.print("Игровое поле:\n\t");
        for (int j = 0; j < board.getSize(); ++j) {
            System.out.printf("%c  ", 'A'+j);
        }
        System.out.println();

        //определяем включены ли подскази из параметров
        boolean hints = findParameter("подсказки").getValue().equals("вкл");

        for (int i = 0; i < board.getSize(); ++i) {
            System.out.printf("%d\t", i+1);
            for (int j = 0; j < board.getSize(); ++j) {
                int[] cell = {i, j};
                Player currentPlayer = players.get(currentPlayerIndex);
                if (hints && currentPlayer.isPlayer() && playerCanSelectCell(currentPlayerIndex, cell)) {
                    System.out.print("*  ");
                } else {
                    System.out.printf("%c  ", board.getCharAt(i, j));
                }
            }
            System.out.println();
        }
    }

    //функция выводит текущий счёт игрока
    private void printPlayersScore() {
        for (Player player: players) {
            System.out.printf("%s:%d\t", player.getName(), player.getScore());
        }System.out.println();
    }

    //функция выводит игру (доску и счет игроков)
    private void printGame() {
        System.out.println();
        printBoard();
        printPlayersScore();
    }

    //проверка конца игры
    //игра не окончена если хотя бы один игрок может ходить
    public boolean isGameEnd() {
        for (int playerId = 0; playerId < players.size(); ++playerId) {
            if (playerCanMove(playerId)) {
                return false;
            }
        }
        return true;
    }

    //функция обновляет список игроков в соответствие с параметрами
    public void resetPlayers() {
        currentPlayerIndex = 0;
        players.clear();
        String mode = findParameter("режим").getValue();

        String nick1 = findParameter("ник1").getValue();
        String nick2 = findParameter("ник2").getValue();
        boolean isBot1 = mode.contains("ИИ против");
        boolean isBot2 = mode.contains("против ИИ");

        players.add(new Player(this, 0, nick1, 'O', board, isBot1));
        players.add(new Player(this, 1, nick2, 'X', board, isBot2));

        for (Player player: players) {
            player.resetScore();
        }
    }

    //функция проверяет, установлен ли новый рекор среди всех игроков(боты не учитываются)
    public void checkBestPlayer() {
        Parameter bestPlayerScoreParameter = findParameter("лучший игрок");
        boolean newBest = false;
        for (Player player: players) {
            if (player.isPlayer() && player.getScore() > Integer.parseInt(bestPlayerScoreParameter.getValue().split(" ")[1])) {
                newBest = true;
                bestPlayerScoreParameter.setValue(player.getName() + " " + player.getScore());
            }
        }

        if (newBest) {
            System.out.println("Новый рекорд!");
            saveSettings();
        }
    }

    //функция проверяет может ли игрок сделать ход в клетку cell
    public boolean playerCanSelectCell(int playerId, int[] cell) {
        if (!board.cellFree(cell)) {
            return false;
        }

        Player currentPlayer = players.get(playerId);           //определяем игрока
        Player otherPlayer = players.get((playerId+1)%2);       //определяем соперника

        //проверяем все направления
        for (int direction = 0; direction < board.getDirectionsCount(); ++direction) {
            if (board.closedCellCount(currentPlayer, otherPlayer, cell, direction) > 0) {
                return true;
            }
        }
        return false;
    }

    //функция проверяет может ли игрок сделать ход
    private boolean playerCanMove(int playerIndex) {
        for (int i = 0; i < board.getSize(); ++i) {
            for (int j = 0; j < board.getSize(); ++j) {
                if (playerCanSelectCell(playerIndex, new int[] {i, j})) {
                    return true;
                }
            }
        }
        return false;
    }

    //функция производит ход игрока
    public void playerMove(int playerId) throws BoardOutException, PlayerCantSelectCellException {
        Player currentPlayer = players.get(playerId);           //определяем игрока
        Player otherPlayer = players.get((playerId+1)%2);       //определяем соперника
        int[] cell = players.get(playerId).getMove();           //определяем ход игрока

        //если клетка выходит за пределы доски
        if (!board.cellExist(cell)) {
            throw new BoardOutException();
        }

        //проверяем на возможность выбора клетки
        if (!board.cellFree(cell)) {
            throw new PlayerCantSelectCellException("клетка уже занята");
        }
        if (!playerCanSelectCell(playerId, cell)) {
            throw new PlayerCantSelectCellException("клетка не закрывает последовательность соперника");
        }

        //меняем поле в соответствие с ходом
        //проверяем все направления
        for (int direction = 0; direction < board.getDirectionsCount(); ++direction) {

            //ищем ближайшую клетку не принадлежащую противнику в этом направлении
            int k = 1;
            while (board.cellBelong(board.nextCell(cell, direction, k), otherPlayer)) {++k;}

            //если она принадлежит текущему игроку, значит он закрывает последовательность противника
            if (k != 1 && board.cellBelong(board.nextCell(cell, direction, k), currentPlayer)) {

                //меняем все замкнутые фишки
                for (k = 1; board.cellBelong(board.nextCell(cell, direction, k), otherPlayer); ++k) {
                    board.setCellChar(board.nextCell(cell, direction, k), currentPlayer.getMoveChar());
                    currentPlayer.incScore();
                    otherPlayer.decScore();
                }
            }
        }

        board.setCellChar(cell, currentPlayer.getMoveChar());
        currentPlayer.incScore();
    }

    //функция начала игры. Заключается в заходе в главное меню игры
    public void run() {
        goToRoom(0);
    }

    //функция осуществляет заход в комнату меню
    public void goToRoom(int roomId) {
        Room currentRoom = menu.get(roomId);
        int cmd = currentRoom.getCommand();

        if (cmd == 0) {
            if (roomId != 0) {
                goToRoom(0);
            } else {
                System.out.println("Выход из игры...");
            }
        } else if (roomId > 0) {
            Parameter parameter = currentRoom.getParameters().get(cmd- currentRoom.getHelp().size());
            if (parameter instanceof DynamicParameter) {
                ((DynamicParameter) parameter).selectValue();
                saveSettings();
            }
            goToRoom(roomId);

        } else if (roomId == 0) {
            switch (cmd) {
                case 1:
                    play();
                    goToRoom(0);
                    break;
                case 2:
                    goToRoom(1);
                    break;
                case 3:
                    goToRoom(2);
                    break;
                default:
                    break;
            }
        }
    }

    //основная функция, запускающая игровой процесс
    public void play() {
        resetPlayers();

        //приводим доску к начальной позиции
        board.fieldClear();

        //устанавливаем фишки игроков по центру
        int mid = board.getSize()/2;
        board.setCellChar(new int[]{mid-1, mid-1}, players.get(0).getMoveChar());
        board.setCellChar(new int[]{  mid,   mid}, players.get(0).getMoveChar());
        board.setCellChar(new int[]{mid-1,   mid}, players.get(1).getMoveChar());
        board.setCellChar(new int[]{  mid, mid-1}, players.get(1).getMoveChar());
        printGame();

        while (true) {
            //проверяем на конец игры
            if (isGameEnd()) {
                int winnerId = 0;
                for (int playerId = 0; playerId < players.size(); ++playerId) {
                    if (players.get(playerId).getScore() > players.get(winnerId).getScore()) {
                        winnerId = playerId;
                    }
                }
                Player winner = players.get(winnerId);

                System.out.println("Игра окончена.");
                System.out.printf("Победил: %s\n", winner.getName());
                checkBestPlayer();
                return;
            }

            Player currentPlayer = players.get(currentPlayerIndex);
            System.out.printf("Ход игрока %s ('%c').\n", currentPlayer.getName(), currentPlayer.getMoveChar());


            if (playerCanMove(currentPlayerIndex)) {
                while (true) {
                    currentPlayer.makeMove();
                    if (currentPlayer.getMove()[0] == -1) {
                        return;
                    }
                    try {
                        playerMove(currentPlayerIndex);
                    } catch (BoardOutException | PlayerCantSelectCellException e) {
                        System.out.println(e.getMessage());
                        continue;
                    }
                    break;
                }
            } else {
                System.out.println("У игрока нет возможности ходить.\nИгрок пропускает ход.");
            }

            currentPlayerIndex = (currentPlayerIndex+1) % players.size();
            printGame();
        }
    }

    //функция сохраняет все параметры игры
    public void saveSettings() {
        try {
            FileWriter settingsFile = new FileWriter("settings.in", false);
            for (Room curRoom : menu) {
                for (Parameter parameter: curRoom.getParameters()) {
                    settingsFile.write(parameter.getName() + ';' + parameter.getValue() + '\n');
                }
            }
            settingsFile.close();
        } catch (IOException ignored) {
        }
    }

    //функция подгружает все параметры игры
    public void loadSettings() {
        try {
            File file = new File("settings.in");
            //создаем объект FileReader для объекта File
            FileReader fr = new FileReader(file);
            //создаем BufferedReader с существующего FileReader для построчного считывания
            BufferedReader reader = new BufferedReader(fr);
            // считаем сначала первую строку
            for (Room currentRoom : menu) {
                for (Parameter parameter: currentRoom.getParameters()) {
                    String[] line = reader.readLine().split(";");
                    if (parameter.getName().equals(line[0])) {parameter.setValue(line[1]);}
                }
            }

        } catch (Exception ignored) {
        }
    }
}
