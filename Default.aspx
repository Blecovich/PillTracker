<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="TreatmentTracker.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Schedule</title>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/5.0.0-alpha2/js/bootstrap.min.js"></script>
    <script src="https://unpkg.com/sweetalert/dist/sweetalert.min.js"></script>
    <link href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        body {
            background-color: #27472A;
        }

        .card {
            background-color: #f0f8ff;
        }

        .hidden {
            display: none;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:HiddenField ID="awake" value="true" runat="server" />
        <div class="container-fluid">
            <div class="row m-3" style="margin-top: 20px;">
                <div class="col-sm-6">
                    <asp:Button runat="server" CssClass="btn btn-success" ID="btnWakeUp" Text="He's Awake!" Style="width: 100%;" OnClick="awake_onclick" />
                </div>
                <div class="col-sm-6">
                    <asp:Button runat="server" CssClass="btn btn-primary" ID="btnSleep" Text="... He's Asleep ..." Style="width: 100%;" OnClick="asleep_onclick" />
                </div>
            </div>
            <div class="row" style="margin-top: 20px;">
                <div class="col-sm-2"></div>
                <div class="col-sm-8 cardholder">
                    <asp:Repeater runat="server" ID="rptSchedule">
                        <HeaderTemplate>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <div class="card mb-1<%# 
                                        ((DateTime)Eval("TreatmentTime") - DateTime.Now).TotalMinutes <= -20 ?
                                        " bg-danger" :  
                                            ((DateTime)Eval("TreatmentTime") - DateTime.Now).TotalMinutes <= 20 ?
                                            " bg-warning" : ""
                                   %>">
                                <div class="card-body">
                                    <h3 class="card-title">
                                        <%# Convert.ToDateTime(Eval("TreatmentTime")).ToShortTimeString() %>
                                    </h3>
                                    <span class="font-weight-bold treatmentName"><%# Eval("TreatmentName") %></span>
                                    <span class="hidden scheduleID"><%# Eval("Id") %></%#></span
                                    <span><%# Eval("TreatmentTime") %></span>
                                </div>
                            </div>
                        </ItemTemplate>
                        <FooterTemplate>
                        </FooterTemplate>
                    </asp:Repeater>
                </div>
                <div class="col-sm-2"></div>
            </div>
        </div>
    </form>
</body>

<script src="https://code.jquery.com/jquery-3.5.1.js"></script>
<script type="text/javascript">
    $(document).ready(function () {

        $('.card').on('click', function (event) {
            console.log(event.currentTarget.parentElement);
            let id = event.target.parentElement.querySelector('.scheduleID').innerText;
            console.log(id);
            console.log("/API.aspx?action=administer&id=" + id);
            $.ajax({
                url: "/API.aspx?action=administer&id=" + id,
                success: function (result) {
                    swal({
                        text: event.target.parentElement.querySelector('.treatmentName').innerText + ' ' + result,
                        button: {
                            classname: "btn btn-success",
                        },
                    },
                    ).then(function (inputValue) {
                        $('#btnWakeUp').click();
                    });
                }
            });
            
        });
    });
</script>
</html>
