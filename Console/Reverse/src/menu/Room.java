package menu;

import menu.parameters.Parameter;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.Scanner;

public class Room {
    private String name;                        //название комнады
    private ArrayList<String> help;             //список команд
    private ArrayList<Parameter> parameters;    //список параметров

    public Room(String _name, String commands) {
        name = _name;
        help = new ArrayList<>();
        help.addAll(Arrays.asList(commands.split(";")));
        parameters = new ArrayList<>();
    }

    //функция доступа к возможным командам
    public ArrayList<String> getHelp() {
        return help;
    }

    //функция доступа к параметрам
    public ArrayList<Parameter> getParameters() {
        return parameters;
    }

    //функция отображает меню и запрашивает у пользователя команду и возвращает ее
    public int getCommand() {
        while (true) {
            System.out.println("\n" + name + ":");
            int item = 0;
            for (int i = 0; i < help.size(); ++i, ++item) {
                System.out.printf("\t%d-\t%s\n", item, help.get(i));
            }
            for (int i = 0; i < parameters.size(); ++i, ++item) {
                System.out.printf("\t%d-\t%s\n", item, parameters.get(i).toString());
            }
            System.out.print("\tВведите команду: ");
            Scanner scanner = new Scanner(System.in);
            int cmd = Integer.parseInt(scanner.next());
            if (cmd < 0 || cmd >= help.size()+parameters.size()) {
                System.out.println("Неизвестная команда!");
            } else {
                return cmd;
            }
        }
    }

    //функция добавляет параметр к списку
    public void addParameter(Parameter parameter) {
        parameters.add(parameter);
    }
}
