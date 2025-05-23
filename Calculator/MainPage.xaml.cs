using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using System.Globalization;

namespace Calculator;

public partial class MainPage : ContentPage, INotifyPropertyChanged
{
    private CancellationTokenSource? delCts;
    public string Input = "0";
    
    public class HistoryItem
    {
        public string Expression { get; set; }
        public double Result { get; set; }
    }
    
    List<HistoryItem> history = new List<HistoryItem>();
    

    
    private int Precedence(string op)
    {
        return op switch
        {
            "+" or "-" => 1,
            "*" or "/" => 2,
            _ => 0
        };
    }

    
    public MainPage()
    {
        InitializeComponent();
        BindingContext = this;
        
    }

    private void RenewEntry()
    {
        if (!InputEntry.IsVisible)
        {
            InputEntry.IsVisible = true;
            ExpressionLabel.IsVisible = false;
            ResultLabel.IsVisible = false;
            
            InputEntry.Text = "";
        }
    }
    
    private void BtnOB_OnClicked(object? sender, EventArgs e)
    {
        RenewEntry();
        InputEntry.Text += "(";
    }

    private void BtnCB_OnClicked(object? sender, EventArgs e)
    {
        RenewEntry();
        InputEntry.Text += ")";
    }

    private void BtnADD_OnClicked(object? sender, EventArgs e)
    {
        RenewEntry();
        InputEntry.Text += "+";
    }

    private void Btn1_OnClicked(object? sender, EventArgs e)
    {
        RenewEntry();
        InputEntry.Text += "1";
    }
    private void Btn2_OnClicked(object? sender, EventArgs e)
    {
        RenewEntry();
        InputEntry.Text += "2";
    }
    private void Btn3_OnClicked(object? sender, EventArgs e)
    {
        RenewEntry();
        InputEntry.Text += "3";
    }

    private void BtnDIV_OnClicked(object? sender, EventArgs e)
    {
        RenewEntry();
        InputEntry.Text += "/";
    }

    private void Btn4_OnClicked(object? sender, EventArgs e)
    {
        RenewEntry();
        InputEntry.Text += "4";
    }

    private void Btn5_OnClicked(object? sender, EventArgs e)
    {
        RenewEntry();
        InputEntry.Text += "5";
    }

    private void Btn6_OnClicked(object? sender, EventArgs e)
    {
        RenewEntry();
        InputEntry.Text += "6";
    }

    private void BtnMUL_OnClicked(object? sender, EventArgs e)
    {
        RenewEntry();
        InputEntry.Text += "*";
    }

    private void Btn7_OnClicked(object? sender, EventArgs e)
    {
        RenewEntry();
        InputEntry.Text += "7";
    }

    private void Btn8_OnClicked(object? sender, EventArgs e)
    {
        RenewEntry();
        InputEntry.Text += "8";
    }

    private void Btn9_OnClicked(object? sender, EventArgs e)
    {
        RenewEntry();
        InputEntry.Text += "9";
    }

    private void BtnMINUS_OnClicked(object? sender, EventArgs e)
    {
        RenewEntry();
        InputEntry.Text += "-";
    }

    private void Btn0_OnClicked(object? sender, EventArgs e)
    {
        RenewEntry();
        InputEntry.Text += "0";
    }

    private void BtnDOT_OnClicked(object? sender, EventArgs e)
    {
        RenewEntry();
        InputEntry.Text += ".";
    }
    
    
    
    private void InputEntry_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        Input = e.NewTextValue;
        Debug.WriteLine("input: " + Input);
    }

    private void InputEntry_OnCompleted(object? sender, EventArgs e)
    {
        string expression = InputEntry.Text;

        try
        {
            double result = Calculate(expression); // реалізуй цю функцію нижче

            // Приховуємо поле вводу
            InputEntry.IsVisible = false;

            // Показуємо оброблений вираз сірим кольором
            ExpressionLabel.Text = expression + " =";
            ExpressionLabel.IsVisible = true;

            // Показуємо результат
            ResultLabel.Text = result.ToString();
            ResultLabel.IsVisible = true;
            
            AddToHistory(expression, result);
        }
        catch (Exception ex)
        {
            DisplayAlert("Помилка", ex.Message, "ОК");
        }
    }
    
    private double Calculate(string expression)
    {
        
        try
        {
            List<string> tokens = Tokenize(expression);
            List<string> rpn = ToRPN(tokens);
            return EvaluateRPN(rpn);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Помилка: " + ex.Message);
            return double.NaN; // або -1, або кидати далі виняток
        }
    }

    private List<string> Tokenize(string expr)
    {
        List<string> tokens = new List<string>();
        int i = 0;

        while (i < expr.Length)
        {
            char c = expr[i];

            if (char.IsWhiteSpace(c))
            {
                i++;
                continue;
            }

            if ("+-*/()".Contains(c))
            {
                if (c == '-' && (i == 0 || "*/+-( ".Contains(expr[i - 1])))
                {
                    string number = "-";
                    i++;
                    while (i < expr.Length && (char.IsDigit(expr[i]) || expr[i] == '.'))
                    {
                        number += expr[i++];
                    }
                    tokens.Add(number);
                }
                else
                {
                    tokens.Add(c.ToString());
                    i++;
                }
            }
            
            else if (char.IsDigit(c) || c == '.')
            {
                string number = "";
                while (i < expr.Length && (char.IsDigit(expr[i]) || expr[i] == '.'))
                {
                    number += expr[i++];
                }
                tokens.Add(number);
            }
            else
            {
                throw new Exception($"Недопустимий символ: '{c}'");
            }
        }

        return tokens;
    }

    private List<string> ToRPN(List<string> tokens)
    {
        List<string> output = new List<string>();
        Stack<string> ops = new Stack<string>();

        foreach (string token in tokens)
        {
            // if (double.TryParse(token, out _))
            if (double.TryParse(token, NumberStyles.Any, CultureInfo.InvariantCulture, out double num))
            {
                output.Add(token);
            }
            else if (token == "(")
            {
                ops.Push(token);
            }
            else if (token == ")")
            {
                while (ops.Count > 0 && ops.Peek() != "(")
                    output.Add(ops.Pop());

                if (ops.Count == 0 || ops.Peek() != "(")
                    throw new Exception("Незбалансовані дужки.");
                ops.Pop();
            }
            else if ("+-*/".Contains(token))
            {
                while (ops.Count > 0 && Precedence(ops.Peek()) >= Precedence(token))
                    output.Add(ops.Pop());

                ops.Push(token);
            }
        }

        while (ops.Count > 0)
        {
            string op = ops.Pop();
            if (op == "(" || op == ")")
                throw new Exception("Незбалансовані дужки.");
            output.Add(op);
        }

        return output;
    }
    private double EvaluateRPN(List<string> rpn)
    {
        Stack<double> stack = new Stack<double>();

        foreach (var token in rpn)
        {
            // if (double.TryParse(token, out double num))
            if (double.TryParse(token, NumberStyles.Any, CultureInfo.InvariantCulture, out double num))
            {
                stack.Push(num);
            }
            else
            {
                if (stack.Count < 2)
                    throw new Exception("Недостатньо операндів.");

                double b = stack.Pop();
                double a = stack.Pop();

                switch (token)
                {
                    case "+": stack.Push(a + b); break;
                    case "-": stack.Push(a - b); break;
                    case "*": stack.Push(a * b); break;
                    case "/":
                        if (b == 0)
                            throw new DivideByZeroException("Ділення на нуль.");
                        stack.Push(a / b);
                        break;
                    default:
                        throw new Exception($"Невідома операція: {token}");
                }
            }
        }

        if (stack.Count != 1)
            throw new Exception("Помилка при обчисленні.");

        return stack.Pop();
    }

    private async void BtnDEL_OnPressed(object? sender, EventArgs e)
    {
        RenewEntry();
        delCts = new CancellationTokenSource();
        var token = delCts.Token;

        try
        {
            await Task.Delay(300, token);
            InputEntry.Text = "";
        }
        catch (TaskCanceledException)
        {
            // Ignore, means released before 300ms
        }
    }

    private void BtnDEL_OnReleased(object? sender, EventArgs e)
    {
        RenewEntry();
        if (delCts != null)
        {
            if (!delCts.IsCancellationRequested)
            {
                delCts.Cancel();
                if (!string.IsNullOrEmpty(InputEntry.Text))
                {
                    InputEntry.Text = InputEntry.Text.Remove(InputEntry.Text.Length - 1);
                }
            }
            delCts.Dispose();
            delCts = null;
        }
    }
    


    void AddToHistory(string expression, double result)
    {
        history.Add(new HistoryItem { Expression = expression, Result = result });
    }
    void UpdateHistoryDisplay()
    {
        HistoryStack.Children.Clear();

        foreach (var item in history)
        {
            var expressionLabel = new Label
            {
                Text = item.Expression,
                TextColor = Colors.Gray,
                FontSize = 15,
                HorizontalTextAlignment = TextAlignment.End
            };

            var resultLabel = new Label
            {
                Text = item.Result.ToString(),
                TextColor = Colors.White,
                FontSize = 18,
                FontAttributes = FontAttributes.Bold,
                HorizontalTextAlignment = TextAlignment.End
            };

            var group = new StackLayout
            {
                Spacing = 0,
                Margin = new Thickness(0, 0, 0, 10),
                Children = { expressionLabel, resultLabel }
            };

            HistoryStack.Children.Add(group);
        }
    }

    private void ClearHistoryButtonClicked(object sender, EventArgs e)
    {
        history.Clear();
        HistoryStack.Children.Clear();
    }
    private void OnHistoryButtonClicked(object sender, EventArgs e)
    {
        HistoryPanel.IsVisible = !HistoryPanel.IsVisible;
        UpdateHistoryDisplay();
    }
}