# opio
opio .net command system (unity, godot.net, any .net project)

```//demo class
class demo
{
    //for print hello world write this
    Console.Write>"hello world"

    //for get class
    {class_name}<

    //for invoke action
    {get_action}>{get_vars}

    //for make variable
    object {name}

    //for make variable and set of
    object {name} = {value}

    //for set variable
    {get_variable} = {new_value}

    //for get variable
    {get_variable}<
}

//for make class
class {class_name}
{
    {code_in_class}
}

//for make class in class
//class 1
class {class_name_one}
{
    //class 2
    class {class_name_one}
    {
        //class 3
        class {class_name_one}
        {
            //code of class 3
        }
    }
}```
