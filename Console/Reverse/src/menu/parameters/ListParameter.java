package menu.parameters;

import java.util.Scanner;

//класс параметр, у которого есть список принимаемых значений
public class ListParameter extends DynamicParameter {

    public ListParameter(String _name, String[] _values) {
        super(_name, _values);
    }

    @Override
    public void selectValue() {
        int cmd;

        for (int i = 0; i < values.length; ++i) {
            System.out.printf("\t%d-%s\t%s\n", i+1, value.equals(values[i])? "*": "", values[i]);
        }
        Scanner scanner = new Scanner(System.in);

        while (true) {
            System.out.print("\tВыберите значение: ");
            cmd = Integer.parseInt(scanner.next()) - 1;
            if (cmd < 0 || cmd >= values.length) {
                System.out.println("\tУкажите значение из списка!");
            } else {
                break;
            }
        }
        value = values[cmd];
    }
}
