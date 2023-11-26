<%@ Page Title="" Language="C#" MasterPageFile="~/AdminC/Admin.Master" AutoEventWireup="true" CodeBehind="ChangePassword_ASIC1.aspx.cs" Inherits="comdeeds.AdminC.ChangePassword_ASIC1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
	<div class="col-md-6 col-md-offset-3">
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
</asp:Content>
