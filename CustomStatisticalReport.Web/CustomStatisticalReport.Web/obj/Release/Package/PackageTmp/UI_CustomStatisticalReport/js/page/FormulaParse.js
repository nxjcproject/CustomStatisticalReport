//公式解析
function FormulaParser(resFormula) {
    var s_formula = resFormula;
    var r_array = [];
    var s_factory = "";
    var i_length = s_formula.length;
    for (var i = 0; i < i_length; i++) {
        var m_char = s_formula.charAt(i);
        if (m_char == '+' || m_char == '-' || m_char == '*' || m_char == '/' || m_char == '%' ||
             m_char == '(' || m_char == ')' || m_char == '[' || m_char == ']' || m_char == '{' || m_char == '}') { //处理运算符
            if (s_factory == "")
                continue;
            else {
                r_array.push(s_factory);//因子入数组
                s_factory = "";
            }
        }
        else if (m_char == " ") { //跳过空格
            continue;
        }
        else { //普通情况
            s_factory += m_char;
        }
    }
    //处理最后一个因子
    if (s_factory != "") {
        r_array.push(s_factory);
        s_factory = "";
    }
    return r_array;
}