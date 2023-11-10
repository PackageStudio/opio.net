using System;
using opio;

namespace opio.demos;
public class Program
{
  public static void Main()
  {
    //for set up basic Systems like string, int, float
    OpioBasic.ApplyBasicInfo();
    
    //for run code
    OpioEngine.StartOpioCommands("//code");

    //for run one command
    OpioEngine.StartOpioCommand("//Command");

    //for give object re
    OpioEngine.GiveOpioObject(/*int of object*/);

    
    //make class in opio
    OpioClass a = new OpioClass{
      Name = "//name"
    };

    //for add child|object to class
    a.objects.Add("//Child name", "object");
  }
}
