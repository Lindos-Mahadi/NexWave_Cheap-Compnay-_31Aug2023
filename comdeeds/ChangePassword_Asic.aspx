<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChangePassword_Asic.aspx.cs" Inherits="comdeeds.ChangePassword_Asic" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
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
    <div class="col-md-5 col-md-offset-4">
        <div class="panel panel-default" style=" margin-top: 76px;">
            <div class="panel-heading"> <h1 style="margin: -12px 0px 12px 0px;font-size: 22px;">ASIC COMMUNICATION-Change Password</h1></div>
     <div class="panel-body">
     <div class="col-md-12">
     <div class="panel panel-info" style="display:block;">
  <div class="panel-heading">Change Password</div>
  <div class="panel-body">
  
      <table  style="width:95%;">
          <tr>
              <td>
                  Old Password<br />
                  <asp:TextBox ID="txtoldpass" runat="server"></asp:TextBox>
              </td>
          </tr>
         <tr>
              <td>
               New Password<br />
                  <asp:TextBox ID="txtnewpass" runat="server"></asp:TextBox>
              </td>
          </tr>
          <tr>
              <td><br /><br />
                   <asp:Button ID="btnchange" runat="server" Text="Update with Edge1" 
          CssClass="btn btn-success" onclick="btnchange_Click" style ="text-transform:uppercase;"  />

                      <asp:Button ID="Button1" runat="server" Text="Update with Edge2" 
          CssClass="btn btn-primary" onclick="btnchange1_Click" style ="text-transform:uppercase;"   />
                
              </td>
          </tr>
      </table>
  <asp:Label ID="lblmsg" runat="server" style="FONT-family: verdana;"></asp:Label>


  </div> 
  </div>



    </div></div>
    </div>
    </form>
</body>
</html>
