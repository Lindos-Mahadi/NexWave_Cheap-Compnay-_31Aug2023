<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RA50.aspx.cs" Inherits="comdeeds.RA50" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>COMDEEDS RA50</title>
     <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css">
  <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.1.1/jquery.min.js"></script>
  <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js"></script>
</head>
<body>
    <form id="form1" runat="server">
    <br />
  <center><img src="Capture.PNG" /></center>  
    <div class="col-md-10 col-md-offset-1">
    <div class="col-md-12">
      <h1>Request withdrawal of form 201 </h1>
    </div>
  <div class="col-md-5" >
  <div class="panel panel-info">
  <div class="panel-heading">Company Details</div>
  <div class="panel-body">
      <asp:HiddenField ID="hdncompanyid" runat="server" Value="1193" />
      	Proposed company name
      <asp:TextBox ID="txtcompanyname" CssClass="form-control input-sm" runat="server" Text="ROB BANK TESTING 1 PTY LTD"></asp:TextBox>
        	Document number of Form 201
      <asp:TextBox ID="txtdocumentno" CssClass="form-control input-sm" runat="server" Text="0EAJ62625"></asp:TextBox>
     Signed Date
      <asp:TextBox ID="txtdate" CssClass="form-control input-sm" runat="server" Text="2022-03-30"></asp:TextBox>
      <br />
                        <asp:CheckBox ID="CheckBox1" runat="server" Text="I declare that the above Information is True and Correct." Checked="true" Enabled="false" />


      <asp:Button ID="btnRA50" runat="server" Text="RA50" 
          CssClass="btn btn-info btn-block" onclick="btnRA50_Click"   />
      <asp:Button ID="btnreq" runat="server" Text="REQUEST/Response" 
          CssClass="btn btn-info btn-block" onclick="btnreq_Click"   />
      
      

  </div>
  </div>
       
    </div>
  
    <div class="col-md-7">
     <div class="panel panel-success">
  <div class="panel-heading">Response</div>
  <div class="panel-body">
  <asp:Label ID="lblmsg" runat="server" style="FONT-family: verdana;"></asp:Label>
  </div>
  </div>



    </div>
    </div>
    </form>
</body>
</html>
