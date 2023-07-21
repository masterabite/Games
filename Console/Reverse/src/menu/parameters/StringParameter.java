package menu.parameters;

import java.util.Scanner;

//параметр который может принимать любую строку
public class StringParameter extends DynamicParameter {

    public StringParameter(String _name, String _value) {
        super(_name, _value);
    }

    @Override
    public void selectValue() {
        System.out.print("\tВведите новое значение: ");
        Scanner scanner = new Scanner(System.in);
        value = scanner.nextLine();
    }
}
