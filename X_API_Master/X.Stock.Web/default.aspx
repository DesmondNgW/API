<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="X.Web.Form.Test" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>test</title>
    <meta name="keywords" content="test" />
    <meta name="description" content="test" />
    <link rel="stylesheet" href="/css/framework/framework.css" />
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
       
    <script type="text/javascript" src="/seajs/sea-2.3.0.js"></script>
    <script type="text/javascript" src="/seajs/seajs-config.js"></script>
    <script type="text/javascript">
        //seajs.use("../js/page/demo/test", function (func) {
        //    func();
        //});
    </script>
</body>
</html>
