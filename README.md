# Unity实现简单计算器
## 前言
本项目为中山大学软件工程学院3D游戏编程与设计课程作业。

## 概述
本项目基于Unity中GUI对象搭建，计算器的基本逻辑与功能启发自iOS自带计算器的简单版本。

计算器实现了在64位以内浮点数的加减乘除运算、数字正负变换、数字转换百分比、清零这样的基本功能，不支持括号的使用。

![计算器界面](https://github.com/L1ttlepsycho/3D_Lab2/blob/main/img_calculater.png)

## 实现方案
在 ```OnGUI()```内进行计算器的实现；```Start()```中进行初始化过程，```OnGUI()```中实现“更新”过程。
### UI
使用```GUI.Box()```构造计算器主框架；

使用```GUI.TextArea()```实现计算器数值和算式的显示；

使用```GUI.Button()```实现计算器各个按键的显示；

### 输入
使用```GUI.Button()```实现的各个按键进行数字、运算符以及其他功能的输入；

### 输出
使用```GUI.TextArea()```将计算器计算结果和算式显示在文本框内；

### 算法实现
使用栈储存运算符和数值，通过栈进出的方式实现乘除高优先级，加减低优先级。

## 具体代码
### 参数定义
全部使用静态参数，由于```OnGui()```函数在运行过程中会被多次调用。

数组```pad_num```用于存储数字键上的数字；

```float_flag```: 表示是否为小数部分

```clear_flag```: 代表一次数字输入是否结束

```ac_flag```: 代表一个算式的结束（AC或者等号输入）

```buffer```: 用于暂时存储输入的数值

```pow```: 辅助读入小数部分，代表幂大小

```display_res```:  存储显示出来的数值

```display_line```: 存储显示出来的算式

```num_buff```: 存储数值的栈

```op_buff ```: 存储运算符的栈
```
    public static int[] pad_num = {1,2,3,4,5,6,7,8,9,0 };

    public static bool float_flag, clear_flag,ac_flag;

    public static double buffer,pow;

    public static string display_res,display_line;

    public static Stack<double> num_buff=new Stack<double>();
    public static Stack<char> op_buff=new Stack<char>();
```
### 实现函数 
初始化
```
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
```    
加减乘除运算
```
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
```
初始化与数值化有关的参数
```
    void reset_flag()
    {
        buffer = 0;
        pow = 0.1;
        float_flag = false;
        clear_flag = false;
        ac_flag = false;
    }
```

### 主函数```OnGUI()```主要逻辑实现

清零键清零重新初始化
```
if (GUI.Button(new Rect(210, 100, 75, 60),"AC")) Init();
```
点击等号，清空数字栈进行计算
```
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
```
数字读入逻辑，将字符串转化为数字
```
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
```
输入小数点
```
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
```
数字变化正负
```
if (GUI.Button(new Rect(285, 100, 75, 60), "+/-"))
{
    if (ac_flag) display_line = "";
    buffer = -buffer;
    display_res = buffer.ToString();
    clear_flag = true;
}
```
数值变为百分比数值
```
if (GUI.Button(new Rect(360, 100, 75, 60), "%"))
{
    if (ac_flag) display_line = "";
    buffer *=0.01;
    display_res = buffer.ToString();
    clear_flag = true;
}
```
加减乘除操作，以加、乘为例
```
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
```

## 段位展示(手动滑稽)
![演示效果](https://github.com/L1ttlepsycho/3D_Lab2/blob/main/Hello%20world%20-%20SampleScene%20-%20Windows%20Mac%20Linux%20-%20Unity%202021.3.8f1c1%20Personal_%20_DX11_%202022-10-06%2017-26-10.gif)
