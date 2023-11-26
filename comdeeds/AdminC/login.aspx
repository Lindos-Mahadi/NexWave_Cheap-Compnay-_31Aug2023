<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="comdeeds.AdminC.login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
      <title>Admin Login</title>
    <link rel="stylesheet" type="text/css" href="../Areas/Admin/assets/lib/perfect-scrollbar/css/perfect-scrollbar.min.css" />
    <link rel="stylesheet" type="text/css" href="../Areas/Admin/assets/lib/material-design-icons/css/material-design-iconic-font.min.css" />
    <!--[if lt IE 9]>
    <script src="https://oss.maxcdn.com/html5shiv/3.7.2/html5shiv.min.js"></script>
    <script src="https://oss.maxcdn.com/respond/1.4.2/respond.min.js"></script>
    <![endif]-->


    <link rel="stylesheet" href="../Areas/Admin/assets/css/style.css" type="text/css" />
    <link rel="stylesheet" href="../Areas/Admin/assets/css/grid.min.css" type="text/css" />
    <link rel="stylesheet" href="../Areas/Admin/assets/css/admin.css" type="text/css" />

    <script src="../Areas/Admin/assets/lib/jquery/jquery.min.js" type="text/javascript"></script>
    <script src="../Areas/Admin/assets/js/grid.min.js" type="text/javascript"></script>
    <script src="../Areas/Admin/assets/js/knockout-3.4.2.js" type="text/javascript"></script>
 <%--   <script src="~/Areas/Admin/assets/js/knockout-validation.js" type="text/javascript"></script>--%>

    <script src="../Areas/Admin/assets/js/knockout-validation.js" type="text/javascript"></script>
    <script>
        var ElePager = {
            limit: 10,
            sizes: [10, 50, 100],
            leftControls: [
                    $('<div title="First" data-role="page-first" class="mdi mdi-arrow-left pager-icon" aria-hidden="true"></div>'),
                    $('<div title="Previous" data-role="page-previous" class="mdi mdi-chevron-left pager-icon" aria-hidden="true"></div>'),
                    $('<div> Page </div>'),
                    $('<div></div>').append($('<input type="text" data-role="page-number" class="form-control pager-input" value="0">')),
                    $('<div>of </div>'),
                    $('<div data-role="page-label-last" style="margin-right: 5px;">0</div>'),
                    $('<div title="Next" data-role="page-next" class="mdi mdi-chevron-right pager-icon" aria-hidden="true"></div>'),
                    $('<div title="Last" data-role="page-last" class="mdi mdi-arrow-right pager-icon" aria-hidden="true"></div>'),
                    $('<div title="Reload" data-role="page-refresh" class="mdi mdi-refresh pager-icon" aria-hidden="true"></div>'),
                    $('<div></div>').append($('<select data-role="page-size" class="form-control pager-input"></select>'))
            ],
            rightControls: [
                    $('<div>Displaying records </div>'),
                    $('<div data-role="record-first">0</div>'),
                    $('<div> - </div>'),
                    $('<div data-role="record-last">0</div>'),
                    $('<div> of </div>'),
                    $('<div data-role="record-total">0</div>').css({ "margin-right": "5px" })
            ]
        };
    </script>
<style type="text/css">
     .m_fullheading {
    padding: 2px 0px;
    float: left;
    width: 100%;
    font-weight: 800;
    border-bottom: 4px solid rgba(74, 141, 197, 0.72);
    margin-bottom: 15px;
}
</style>
</head>
<body>
    <form id="form1" runat="server">
    <div><br /><br /><br />
    <div class="col-md-5 col-md-offset-4">
        <div class="panel panel-default">
            <div class="panel-heading">Administrator Login</div>
            <div class="panel-body">
                <table class="table table-bordered">
                    <tr><td><asp:HiddenField ID="hdnid" runat="server" />
                        <asp:Label ID="Label1" runat="server" Text="Admin-Id"></asp:Label></td>
                        <td>
                            <asp:TextBox ID="txtid" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                        </td>

                    </tr>
                     <tr><td><asp:Label ID="Label2" runat="server" Text="Password"></asp:Label></td>
                        <td>
                            <asp:TextBox ID="txtpassword" TextMode="Password" runat="server" CssClass="form-control input-sm"></asp:TextBox>
                        </td>

                    </tr>
                     <tr><td>
                         <asp:Label ID="lblmsg" runat="server" Text="" ForeColor="Red" Font-Size="Small"></asp:Label>

                     </td>
                        <td>
                            <asp:Button ID="btnlogin" runat="server" Text="SignIn" OnClick="btnlogin_Click" CssClass="btn btn-default btn-sm" />
                       &nbsp;&nbsp;&nbsp;     <asp:LinkButton ID="lnkforgot" runat="server" OnClick="lnkforgot_Click"> Forgot Password?</asp:LinkButton>
                        </td>

                    </tr>
                </table>
            </div>
        </div>
    </div>
    </div>
    </form>
</body>
</html>
