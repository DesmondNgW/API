<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="X.Web.Form.Test" %>


<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
    <head runat="server">
        <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
        <title>test</title>
        <meta name="keywords" content="test" />
        <meta name="description" content="test" />
    </head>
    <body>
        <div><%=sbCus.ToString() %></div>

        <table>
            <thead>
                <tr>
                    <th>代码</th>
                    <th>名称</th>
                    <th>成本价</th>
                    <th>当前价</th>
                    <th>涨幅</th>
                    <th>收益</th>
                </tr>
            </thead>
            <tbody>
            <%=sbTable.ToString() %>
            </tbody>
        </table>
    </body>
</html>
