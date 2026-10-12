using System.Globalization;

namespace Backend;

public static class ExpressionEvaluator

{
    public static double Evaluate(string infix) => EvaluatePostfix(ToPostfix(infix));


    private static List<string> ToPostfix(string infix)
    {
        var postfix = new List<string>();
        var stack = new Stack<char>();
        bool expectOperand = true;
        int i = 0;


        while (i < infix.Length)
        {
            char item = infix[i];

            if (char.IsWhiteSpace(item)) { i++; continue; }

        
            bool negativeNumber = item == '-' && expectOperand
                                  && i + 1 < infix.Length
                                  && (char.IsDigit(infix[i + 1]) || infix[i + 1] == '.');

            if (char.IsDigit(item) || item == '.' || negativeNumber)
            {
                int start = i;
                if (negativeNumber) i++;
                while (i < infix.Length && (char.IsDigit(infix[i]) || infix[i] == '.')) i++;

                postfix.Add(infix.Substring(start, i - start));
                expectOperand = false;
                continue;
            }

            if (!IsOperator(item))
                throw new Exception($"Invalid character '{item}'.");

            if (item == ')')
            {
                while (stack.Count > 0 && stack.Peek() != '(')
                    postfix.Add(stack.Pop().ToString());

                if (stack.Count == 0)
                    throw new Exception("Mismatched parentheses.");

                stack.Pop(); 
                expectOperand = false;
            }
            else
            {
               
                while (stack.Count > 0 && PriorityInfix(item) <= PriorityStack(stack.Peek()))
                    postfix.Add(stack.Pop().ToString());

                stack.Push(item);
                expectOperand = true;
            }
            i++;
        }

        while (stack.Count > 0)
        {
            char op = stack.Pop();
            if (op == '(') throw new Exception("Mismatched parentheses.");
            postfix.Add(op.ToString());
        }
        return postfix;
    }


    private static int PriorityStack(char op) => op switch

    {
        '^' => 3,
        '*' => 2,
        '/' => 2,
        '+' => 1,
        '-' => 1,
        '(' => 0,
        _ => throw new Exception("Invalid expression."),
    };



    private static int PriorityInfix(char op) => op switch

    {
        '^' => 4,
        '*' => 2,
        '/' => 2,
        '+' => 1,
        '-' => 1,
        '(' => 5,
        _ => throw new Exception("Invalid expression."),
    };


    private static bool IsOperator(char item) => (item == '^' || item == '*' || item == '/' || item == '+' || item == '-' || item == '(' || item == ')');


    private static double EvaluatePostfix(List<string> postfix)
    {
        var stack = new Stack<double>();
        foreach (var token in postfix)
        {
            if (token.Length == 1 && IsOperator(token[0]))
            {
                if (stack.Count < 2) throw new Exception("Invalidate expression.");
                var ope2 = stack.Pop();
                var ope1 = stack.Pop();
                stack.Push(Calculate(ope1, ope2, token[0]));
            }
            else
            {
                if (!double.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out var value))
                    throw new Exception($"Invalid number '{token}'.");
                stack.Push(value);
            }
        }
        if (stack.Count != 1) throw new Exception("Invalid expression");
        return stack.Pop();
    }

    private static double Calculate(double ope1, double ope2, char item) => item switch
    {
        '^' => Math.Pow(ope1, ope2),
        '*' => ope1 * ope2,
        '/' => ope1 / ope2,
        '+' => ope1 + ope2,
        '-' => ope1 - ope2,
        _ => throw new Exception("Invalid expression."),
    };

}
