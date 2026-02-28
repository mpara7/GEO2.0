using GeoInferenceEngine.EquivalencePlaneGeometry.Models.Exprs.ZExprs;
using GeoInferenceEngine.EquivalencePlaneGeometry.Models.Exprs.ZExprs.MutNodes;

namespace GeoInferenceEngine.EquivalencePlaneGeometry.Models.Exprs.ZExprs
{
    public class ExprTokenizer
    {
        private readonly string _expression;
        private int _position;
        public ExprTokenizer(string expression)
        {
            _expression = expression;
            _position = 0;
        }
        public string NextToken()
        {
            while (_position < _expression.Length && char.IsWhiteSpace(_expression[_position]))
            {
                _position++;
            }

            if (_position == _expression.Length)
            {
                return null;
            }

            if (char.IsDigit(_expression[_position]) || _expression[_position] == '.')
            {
                return ReadNumber();
            }

            if (char.IsLetter(_expression[_position]))
            {
                return ReadIdentifier();
            }

            return _expression[_position++].ToString();
        }

        private string ReadNumber()
        {
            int start = _position;
            while (_position < _expression.Length &&
                   (char.IsDigit(_expression[_position]) || _expression[_position] == '.'))
            {
                _position++;
            }
            return _expression.Substring(start, _position - start);
        }

        private string ReadIdentifier()
        {
            int start = _position;
            while (_position < _expression.Length && char.IsLetterOrDigit(_expression[_position]))
            {
                _position++;
            }
            return _expression.Substring(start, _position - start);
        }
    }

    public class ExprParser
    {
        
        private readonly ExprTokenizer _tokenizer;
        private string _currentToken;
        public ExprParser(string expression)
        {
            _tokenizer = new ExprTokenizer(expression);
            NextToken();
        }

        private void NextToken()
        {
            _currentToken = _tokenizer.NextToken();
        }

        public Expr Parse()
        {
            return ParseExpression();
        }

        private Expr ParseExpression()
        {
            SumNode sum = new SumNode();
            sum.Addends.Add(ParseTerm());

            while (_currentToken == "+" || _currentToken == "-")
            {
                bool isPositive = _currentToken == "+";
                NextToken();
                if (isPositive)
                {
                    sum.Addends.Add(ParseTerm());
                }
                else
                {
                    sum.Addends.Add(ParseTerm());
                }

            }
            return sum.Addends.Count + sum.Subtrahends.Count == 1 ?
                sum.Addends.Count == 1 ? sum.Addends[0] : sum.Subtrahends[0] : sum;
        }

        private Expr ParseTerm()
        {
            ProductNode product = new ProductNode();
            Expr factor = ParseFactor();
            product.Multipliers.Add(factor);

            while (_currentToken == "*" || _currentToken == "/")
            {
                bool isDivision = _currentToken == "/";
                NextToken();
                if (isDivision)
                {
                    product.Divisors.Add(ParseTerm());
                }
                else
                {
                    product.Multipliers.Add(ParseTerm());
                }
            }

            return product.Multipliers.Count + product.Divisors.Count == 1 && product.IsPositive ?
                product.Multipliers.Count == 1 ? product.Multipliers[0] : product.Divisors[0] : product;
        }

        private Expr ParseFactor()
        {
            Expr baseExpr = ParsePrimary(); // 解析基础表达式（数字、变量或括号）  

            while (_currentToken == "^") // 检查是否是指数运算符  
            {
                NextToken();
                Expr exponent = ParseFactor(); // 右结合：递归解析右侧的指数  
                var powerNode = new PowerNode
                {
                    Base = baseExpr,
                    Exponent = exponent
                };
                baseExpr = powerNode; // 更新基础表达式为指数节点  
            }

            return baseExpr;
        }

        private Expr ParsePrimary()
        {
            if (_currentToken == "(")
            {
                NextToken();
                Expr node = ParseExpression();
                if (_currentToken != ")")
                    throw new ArgumentException("Missing closing parenthesis");
                NextToken();
                return node;
            }

            if (_currentToken == "-")
            {
                NextToken();
                ProductNode node = new ProductNode();
                node.IsPositive = false;
                node.Multipliers.Add(ParseTerm());
                return node;
            }

            if (int.TryParse(_currentToken, out int number))
            {
                NextToken();
                return new IntNode(number);
            }

            if (_currentToken == "sin" || _currentToken == "cos" || _currentToken == "tan")
            {

                string function = _currentToken;
                NextToken();
                if (_currentToken != "(")
                    throw new ArgumentException($"Missing opening parenthesis after {function}");
                NextToken();
                Expr argument = ParseExpression();
                if (_currentToken != ")")
                    throw new ArgumentException($"Missing closing parenthesis after {function} argument");
                NextToken();

                if (function == "sin")
                {
                    var node = new SinNode();
                    node.Expr = argument;
                    return node;
                }
                else if (function == "cos")
                {
                    var node = new CosNode();
                    node.Expr = argument;
                    return node;
                }
                else if (function == "tan")
                {
                    var node = new TanNode();
                    node.Expr = argument;
                    return node;
                }
                else if (function == "abs")
                {
                    //var node = new AbsNode();
                    //node.Expr = argument;
                    //return node;
                }
                else if (function == "sqrt")
                {
                    var node = new PowerNode() { Exponent = Expr.AHalf };
                    node.Base = argument;
                    return node;
                }
                throw new ArgumentException();
            }

            if (char.IsLetter(_currentToken[0]))
            {
                string variable = _currentToken;
                NextToken();
                var v = new NameNode(variable);
                NameNode varNode = new NameNode(variable);
                return varNode;
            }

            throw new ArgumentException($"Unexpected token: {_currentToken}");
        }


    }
}
