using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


//简单计算器，接受double类型范围内输入，不支持括号运算符
public class Calculator : MonoBehaviour
{
    public static int[] pad_num = {1,2,3,4,5,6,7,8,9,0 };

    //float_flag: 表示是否为小数部分
    //clear_flag: 代表一次数字输入是否结束
    //ac_flag: 代表一个算式的结束（AC或者等号输入）
    public static bool float_flag, clear_flag,ac_flag;

    //存储数值用的buffer以及用于计算小数输入的pow;
    public static double buffer,pow;

    //display_res：单次输入字符    display_line: 算式
    public static string display_res,display_line;

    //数字栈储存输入数字，符号栈存符号
    public static Stack<double> num_buff=new Stack<double>();
    public static Stack<char> op_buff=new Stack<char>();

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    
    void OnGUI()
    {   
        GUI.Box(new Rect(210, 25, 300,370), "Calculator");

        //显示区域
        GUI.TextArea(new Rect(215, 50, 290, 25), display_line);
        GUI.TextArea(new Rect(215, 75, 290, 25), display_res);
        //清零键清零重新初始化
        if (GUI.Button(new Rect(210, 100, 75, 60),"AC")) Init();

        //点击等号，清空数字栈进行计算
        if (GUI.Button(new Rect(435, 340, 75, 60), "="))
        {
            if (ac_flag) display_line = "";
            num_buff.Push(buffer);
            display_line += buffer.ToString();
            while (op_buff.Count > 0)
            {
                double tmp1 = num_buff.Pop();
                double tmp2 = num_buff.Pop();
                char op = op_buff.Pop();
                num_buff.Push(Calculate( tmp2, tmp1, op));
            }
            buffer = num_buff.Pop();
            display_res = buffer.ToString();
            display_line += "=" + buffer.ToString();
            clear_flag = true;
            ac_flag = true;
        }

        //数字读入逻辑，将字符串转化为数字
        for(int i = 0; i < 10;i++)
        {
            if (i != 9 && GUI.Button(new Rect(210 + i % 3 * 75, 160 + (i / 3 * 60), 75, 60), pad_num[i].ToString()))
            {
                if (ac_flag) display_line = "";
                if (clear_flag)
                {
                    reset_flag();
                    display_res = "";
                }
                if (float_flag) 
                {
                    buffer += pow * pad_num[i];
                    pow /= 10;
                }
                else buffer = buffer * 10 + pad_num[i];
                display_res += pad_num[i].ToString("D");
            }
            if (i == 9 && GUI.Button(new Rect(210, 340, 150, 60), pad_num[i].ToString()))
            {
                if (ac_flag) display_line = "";
                if (clear_flag)
                {
                    reset_flag();
                    display_res = "";
                }
                if (float_flag)
                {
                    buffer += pow * pad_num[i];
                    pow /= 10;
                }
                else buffer = buffer * 10 + pad_num[i];
                display_res += pad_num[i].ToString("D");
            }
        }

        //输入小数点
        if(GUI.Button(new Rect(360, 340, 75, 60), ".")&&!float_flag)
        {
            if (ac_flag)
            {
                display_line = "";
                display_res = buffer.ToString();
            }
            float_flag = true;
            clear_flag = false;
            display_res += ".";
        }

        //数字变化正负
        if (GUI.Button(new Rect(285, 100, 75, 60), "+/-"))
        {
            if (ac_flag) display_line = "";
            buffer = -buffer;
            display_res = buffer.ToString();
            clear_flag = true;
        }

        //数值变为百分比数值
        if (GUI.Button(new Rect(360, 100, 75, 60), "%"))
        {
            if (ac_flag) display_line = "";
            buffer *=0.01;
            display_res = buffer.ToString();
            clear_flag = true;
        }

        //加减乘除操作
        if (GUI.Button(new Rect(435, 100, 75, 60), "+"))
        {
            if (ac_flag) display_line = "";
            if (num_buff.Count == 0)
                num_buff.Push(buffer);
            else
            {
                double tmp = num_buff.Pop();
                char op = op_buff.Pop();
                num_buff.Push(Calculate(tmp, buffer, op));
            }
            op_buff.Push('+');
            reset_flag();
            display_line += display_res + "+";
            display_res = "0";
            clear_flag = true;
        }
        if(GUI.Button(new Rect(435, 160, 75, 60), "-"))
        {
            if (ac_flag) display_line = "";
            if (num_buff.Count == 0)
                num_buff.Push(buffer);
            else
            {
                double tmp = num_buff.Pop();
                char op = op_buff.Pop();
                num_buff.Push(Calculate(tmp, buffer, op));
            }
            op_buff.Push('-');
            reset_flag();
            display_line += display_res + "-";
            display_res = "0";
            clear_flag=true;
        }

        if (GUI.Button(new Rect(435, 220, 75, 60), "X"))
        {
            if (ac_flag) display_line = "";
            num_buff.Push(buffer);           
            op_buff.Push('*');
            reset_flag();
            display_line += display_res + "*";
            display_res = "0";
            clear_flag = true;
        }
        if (GUI.Button(new Rect(435, 280, 75, 60), "/"))
        {
            if (ac_flag) display_line = "";
            num_buff.Push(buffer);
            op_buff.Push('/');
            reset_flag();
            display_line += display_res + "/";
            display_res = "0";
            clear_flag = true;
        }
    }

    //初始化
    void Init()
    {   
        num_buff.Clear();
        op_buff.Clear();
        display_res = "0";
        display_line = "";
        buffer = 0;
        pow = 0.1;
        float_flag = false;
        clear_flag = true;
        ac_flag = false;
    }
    
    //加减乘除运算
    double Calculate(double a,double b, char op)
    {
        switch (op)
        {
            case '+': return a + b;
            case '-': return a - b;
            case '*': return a * b;
            case '/': return a / b;
        }
        return 0;
    }

    //初始化与数值化有关参数
    void reset_flag()
    {
        buffer = 0;
        pow = 0.1;
        float_flag = false;
        clear_flag = false;
        ac_flag = false;
    }
}
