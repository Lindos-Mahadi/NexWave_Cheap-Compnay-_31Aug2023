ko.bindingHandlers.datepicker = {
    init: function (element, valueAccessor, allBindingsAccessor) {
        var value = ko.utils.unwrapObservable(valueAccessor());
        $(element).val(moment(value).format("DD/MM/YYYY"));
        var options = allBindingsAccessor().datepickerOptions || { autoclose: !0, componentIcon: ".sl.sl-icon-calendar", navIcons: { rightIcon: "sl sl-icon-arrow-right", leftIcon: "sl sl-icon-arrow-left" } };
        $(element).datetimepicker(options).on("changeDate", function (ev) {
            var observable = valueAccessor();
            observable(ev.date);
        });
    },
    update: function (element, valueAccessor) {
        var value = ko.utils.unwrapObservable(valueAccessor());
        $(element).datetimepicker("setValue", value);
    }
};

ko.validation.init({
    grouping: {
        deep: true,
        live: true,
        observable: true
    }
}, true);

ko.validation.rules['checked'] = {
    validator: function (value) {
        if (!value) {
            return false;
        }
        return true;
    }
};

ko.validation.rules['validTrustee'] = {
    validator: function (array) {
        var a = ko.utils.arrayFirst(array, function (item) {
            if (typeof item.istrustee === 'function') {
                return item.istrustee() == true;
            } else {
                return false;
            }
        });
        return a;
    },
    message: 'One of the members must be appointed as trustee or a company can act as Trustee.'
};

ko.validation.registerExtenders();

window.onload = function () {
    var applicantdetails = function (fname, lname, email, ps, phone) {
        this.firstname = ko.observable(fname).extend({
            required: true
        });;
        this.lastname = ko.observable(lname).extend({
            required: true
        });;
        this.email = ko.observable(email).extend({
            required: true,
            email: true
        });;
        this.pass = ko.observable(ps).extend({
            required: { onlyIf: function () { return isAuth == "False" } }
        });;
        this.phone = ko.observable(phone).extend({
            required: true
        });;
    };
    var trustdetails = function (id, type, Name, tDate, state, smsf, abn, ptcn, acn, pa, ln, smsfcomp, smsfacn, smsfdate, propertydate, ExistingSetupDate, ClauseNumber) {
        var t = this;
        t.Id = ko.observable(parseInt(id)).extend({
            required: true
        });
        t.TrustType = ko.observable(type).extend({
            required: true
        });
        t.TrustName = ko.observable(Name).extend({
            required: true
        });;
        t.TrustDate = ko.observable(tDate).extend({
            required: true
        });;
        t.TrustState = ko.observable(state).extend({
            required: true
        });;
        t.SMSFCompanyName = ko.observable(smsfcomp).extend({
            required: { onlyIf: function () { return t.TrustType() == "Bare Trust" || t.TrustType() == "Super Fund Trust" } }
        });;
        t.SMSFACN = ko.observable(smsfacn).extend({
            required: { onlyIf: function () { return t.TrustType() == "Bare Trust" || t.TrustType() == "Super Fund Trust" } }
        });;
        t.SMSFSetupDate = ko.observable(smsfdate).extend({
            required: { onlyIf: function () { return t.TrustType() == "Bare Trust" } }
        });;
        t.SMSF = ko.observable(smsf).extend({
            required: { onlyIf: function () { return t.TrustType() == "Bare Trust" } }
        });;
        t.ABN = ko.observable(abn).extend({
            required: { onlyIf: function () { return t.TrustType() == "Bare Trust" } }
        });;
        t.PropertyTrusteeName = ko.observable(ptcn).extend({
            required: { onlyIf: function () { return t.TrustType() == "Bare Trust" } }
        });;
        t.ACN = ko.observable(acn).extend({
            required: { onlyIf: function () { return t.TrustType() == "Bare Trust" } }
        });;
        t.PropertyTrusteeDate = ko.observable(propertydate).extend({
            required: { onlyIf: function () { return t.TrustType() == "Bare Trust" } }
        });;
        t.PropertyAddress = ko.observable(pa).extend({
            required: { onlyIf: function () { return t.TrustType() == "Bare Trust" } }
        });;
        t.LenderName = ko.observable(ln).extend({
            required: { onlyIf: function () { return t.TrustType() == "Bare Trust" } }
        });;
        t.ExistingSetupDate = ko.observable(ExistingSetupDate).extend({
            required: { onlyIf: function () { return t.TrustType == "Super Fund Trust - Update" } }
        });;
        t.ClauseNumber = ko.observable(ClauseNumber).extend({
            required: { onlyIf: function () { return t.TrustType == "Super Fund Trust - Update" } }
        });;
        return t;
    };
    var appointor = function (id, aType, fName, mName, lName, compName, Acn, seal, unit, street, state, suburb, postcode, country, unittype, unitnum, totalamount, amountowe, dob) {
        var a = this;
        a.Id = ko.observable(parseInt(id)).extend({
            required: true
        });;
        a.HolderType = ko.observable(aType).extend({
            required: { onlyIf: function () { return trustDetail().TrustType() == "Unit Trust" } }
        });;
        a.FirstName = ko.observable(fName).extend({
            required: { onlyIf: function () { return a.HolderType() != "company" } }
        });;
        a.MiddleName = ko.observable(mName);
        a.LastName = ko.observable(lName).extend({
            required: { onlyIf: function () { return a.HolderType() != "company" } }
        });;

        a.CompanyName = ko.observable(compName).extend({
            required: { onlyIf: function () { return trustDetail().TrustType() == "Unit Trust" && a.HolderType() == "company" } }
        });;
        a.CompanyACN = ko.observable(Acn).extend({
            required: { onlyIf: function () { return trustDetail().TrustType() == "Unit Trust" && a.HolderType() == "company" } }
        });;
        a.CommanSeal = ko.observable(seal).extend({
            required: { onlyIf: function () { return trustDetail().TrustType() == "Unit Trust" && a.HolderType() == "company" } }
        });;

        a.UnitLevel = ko.observable(unit);
        a.Street = ko.observable(street).extend({
            required: true
        });;
        a.State = ko.observable(state).extend({
            required: true
        });;
        a.Suburb = ko.observable(suburb).extend({
            required: true
        });;
        a.PostCode = ko.observable(postcode).extend({
            required: true
        });;
        a.Country = ko.observable(country).extend({
            required: true
        });;
        a.UnitType = ko.observable(unittype).extend({
            required: { onlyIf: function () { return trustDetail().TrustType() == "Unit Trust" } }
        });;
        a.UnitNumber = ko.observable(unitnum).extend({
            required: { onlyIf: function () { return trustDetail().TrustType() == "Unit Trust" } },
            min: trustDetail().TrustType() == "Unit Trust" ? 1 : 0,
            number: true
        });;
        a.UnitTotalAmount = ko.observable(totalamount).extend({
            required: { onlyIf: function () { return trustDetail().TrustType() == "Unit Trust" } },
            min: trustDetail().TrustType() == "Unit Trust" ? 1 : 0,
            number: true
        });;
        a.UnitAmountOwing = ko.observable(amountowe).extend({
            required: { onlyIf: function () { return trustDetail().TrustType() == "Unit Trust" } },
            min: 0,
            number: true
        });;
        a.dob = ko.observable(dob).extend({
            required: true
        });;
        return a;
    };
    var unitPN = function (price, no) {
        var u = this;
        u.UnitPrice = ko.observable(price).extend({
            required: { onlyIf: function () { return trustDetail().TrustType() == "Unit Trust" } },
            min: 1,
            number: true
        });
        u.NoUnitHolders = ko.observable(no).extend({
            required: true
        });
        u.NoUnitHolders.subscribe(function (n) {
            var atp = trustDetail().TrustType() != "Unit Trust" ? "member" : "";
            var sl = self.UnitHolders().length;
            if (n != "" && parseInt(n) != sl) {
                if (parseInt(n) > sl) {
                    for (var i = sl; i < n; i++) {
                        self.UnitHolders.push(new appointor(0, atp, "", "", "", "", "", "", "", "", "", "", "", "Australia", "Fully paid units", "", "", "", new Date()));
                    }
                } else if (parseInt(n) < sl) {
                    for (var i = n; i < sl; i++) {
                        self.UnitHolders.pop();
                    }
                }
            } else if (!n) {
                self.UnitHolders([]);
            }
        });
        return u;
    };
    var trustees = function (id, name, istrustee) {
        var t = this;
        t.Id = ko.observable(id);
        t.Name = ko.observable(name);
        t.istrustee = ko.observable(istrustee);
        return t;
    };
    var beneficiaryCompany = function (id, cName, cACN, rDate, pName) {
        var b = this;
        b.Id = ko.observable(id).extend({ required: true });
        b.CompanyName = ko.observable(cName).extend({
            required: { onlyIf: function () { return self.trusteesDetails().bType() == "company" } }
        });
        b.CompanyACN = ko.observable(cACN).extend({
            required: { onlyIf: function () { return self.trusteesDetails().bType() == "company" } }
        });
        b.RegDate = ko.observable(rDate).extend({
            required: { onlyIf: function () { return self.trusteesDetails().bType() == "company" } }
        });
        b.ContactPerson = ko.observable(pName).extend({
            required: { onlyIf: function () { return self.trusteesDetails().bType() == "company" } }
        });
        return b;
    };

    var beneficiarydetails = function (p, c, type) {
        var b = this;
        b.Members = ko.observableArray(p);
        b.Company = ko.observableArray(c);
        b.bType = ko.observable(type).extend({
            required: true
        });
        return b;
    };

    function binndviews() {
        var self = this;
        self.tabbing = ko.observable("0");
        self.applicant = ko.observableArray();
        self.trustDetail = ko.observableArray();
        self.UPrice = ko.observableArray();
        self.UnitHolders = ko.observableArray();
        self.ajxproc = ko.observable(false);
        self.setupcost = ko.observable(100);
        self.setupgst = ko.observable(8);
        self.totalcost = ko.observable(108);
        self.trusteesDetails = ko.observableArray();
        self.terr = ko.observable(true);
        self.Issummary = ko.observable(false);

        //Init form
        self.applicant(new applicantdetails(user.fname, user.lname, user.email, "", user.phone));
        self.trustDetail(new trustdetails(0, "", "", new Date(), "", "", "", "", "", "", "", "", "", new Date(), new Date(), new Date(), ""));
        self.trusteesDetails(new beneficiarydetails("", "", "person"));
        console.log(self.trusteesDetails());
        // applicant detail
        self.applicantsubmit = function (f) {
            self.applicantformerror = ko.validation.group(self.applicant(), {
                deep: true
            });

            if (self.applicantformerror().length !== 0) {
                self.applicantformerror.showAllMessages();
            } else {
                if (isAuth == "False") {
                    return true;
                } else {
                    var dt = JSON.stringify(ko.toJS(self.applicant()));
                    $.ajax({
                        type: "POST",
                        beforeSend: function () { self.ajxproc(true); },
                        complete: function () { self.ajxproc(false); },
                        url: "/api/SiteApi/AddTrust/",
                        data: dt,
                        contentType: "application/json",
                        success: function (d) {
                            if (d.msg == "success") {
                                toastr.success("Trust details added successfully");
                                self.toggletab(2);
                            } else {
                                toastr.error("Error occurred on server, please try again.");
                            }
                        },
                        error: function () {
                            toastr.error("Server error");
                        }
                    });
                }
            }
        }

        //Get Trust Details
        self.fetchTrust = function () {
            $.ajax({
                type: "GET",
                beforeSend: function () { self.ajxproc(true); },
                complete: function () { self.ajxproc(false); },
                url: "/api/SiteApi/GetTrustDetails/",
                contentType: "application/json",
                success: function (d) {
                    if (d.msg == "success") {
                        var t = d.trust;

                        self.trustDetail(new trustdetails(
                            t.Id ? t.Id : 0,
                            t.TrustType ? t.TrustType : "",
                            t.TrustName ? t.TrustName : "",
                            moment(t.TrustDate),
                            t.TrustState ? t.TrustState : "", t.Smsf, t.Abn, t.PropertyTrusteeName, t.PropertyTrusteeAcn,
                            t.PropertyAddress, t.LenderName, t.SmsfCompanyName, t.SmsfAcn,
                            moment(t.SmsfCompanySetupDate),
                            moment(t.PropertyTrusteeDate), moment(t.ExistingSetupDate), t.ClauseNumber
                        ));

                        self.tabbing(2);
                    } else if (d.msg == "null") {
                        self.trustDetail(new trustdetails(0, "", "", new Date(), "", "", "", "", "", "", "", "", "", new Date(), new Date(), new Date(), ""));
                        self.tabbing(2);
                    } else {
                        toastr.error("Error occurred on server, please try again.");
                    }
                },
                error: function () {
                    toastr.error("Server error");
                }
            });
        };

        //trust submit
        self.trustsubmit = function (f) {
            self.trustformerror = ko.validation.group(self.trustDetail(), {
                deep: true
            });
            if (self.trustformerror().length !== 0) {
                self.trustformerror.showAllMessages();
            } else {
                //self.trustDetail().TrustDate(moment(self.trustDetail().TrustDate()).format("DD/MM/YYYY"));
                //var dt = JSON.stringify(ko.toJS(self.trustDetail()));
                //self.trustDetail().TrustDate(moment(new Date(moment(self.trustDetail().TrustDate(), 'DD MMM,YYYY'))));
                var d = self.trustDetail();
                var dt = JSON.stringify({
                    Id: d.Id(),
                    TrustDate: moment(d.TrustDate()).format("DD/MM/YYYY"),
                    TrustName: d.TrustName(),
                    TrustState: d.TrustState(),
                    TrustType: d.TrustType(),
                    Smsf: d.SMSF(),
                    Abn: d.ABN(),
                    PropertyTrusteeAcn: d.ACN(),
                    PropertyTrusteeName: d.PropertyTrusteeName(),
                    PropertyAddress: d.PropertyAddress(),
                    LenderName: d.LenderName(),
                    SmsfCompanyName: d.SMSFCompanyName(),
                    SmsfAcn: d.SMSFACN(),
                    SmsfCompanySetupDate: moment(d.SMSFSetupDate()).format("MM/DD/YYYY"),
                    PropertyTrusteeDate: moment(d.PropertyTrusteeDate()).format("MM/DD/YYYY"),
                    ExistingSetupDate: moment(d.ExistingSetupDate()).format("MM/DD/YYYY"),
                    ClauseNumber: d.ClauseNumber()
                });

                $.ajax({
                    type: "POST",
                    beforeSend: function () { self.ajxproc(true); },
                    complete: function () { self.ajxproc(false); },
                    url: "/api/SiteApi/updateTrust/",
                    data: dt,
                    contentType: "application/json",
                    success: function (d) {
                        if (d.msg == "success") {
                            toastr.success("Trust details added successfully");
                            self.toggletab(3);
                        } else if (d.msg == "invalid_date") {
                            toastr.error("Please enter a correct date (Date/Month/Year).");
                        } else {
                            toastr.error("Error occurred on server, please try again.");
                        }
                    },
                    error: function () {
                        toastr.error("Server error");
                    }
                });
            }
        };
        //Trust detail end

        //Get unit holders Details
        self.fetchTrustAppointer = function () {
            $.ajax({
                type: "GET",
                beforeSend: function () { self.ajxproc(true); },
                complete: function () { self.ajxproc(false); },
                url: "/api/SiteApi/GetTrustAppointers/",
                contentType: "application/json",
                success: function (d) {
                    if (d.msg == "success") {
                        var ta = d.trust.appointer;
                        self.UPrice(new unitPN(d.trust.OrdinaryPrice, d.trust.TotalUnitHolders));
                        var a = [];
                        for (var i = 0; i < ta.length; i++) {
                            var t = ta[i];
                            a.push(new appointor(t.Id, t.HolderType, t.FirstName, t.MiddleName, t.LastName, t.CompanyName, t.CompanyAcn, t.CommanSeal, t.UnitLevel, t.Street, t.State, t.Suburb, t.PostCode, t.Country, t.UnitType, t.UnitNumber, t.UnitTotalAmount, t.UnitAmountOwing, new Date()));
                        }
                        self.UnitHolders(a);

                        self.tabbing(3);
                    } else if (d.msg == "null") {
                        self.UnitHolders([]);
                        self.UPrice(new unitPN("", ""));
                        self.tabbing(3);
                    } else {
                        toastr.error("Error occurred on server, please try again.");
                    }
                },
                error: function () {
                    toastr.error("Server error");
                }
            });
        };

        // Unit holders submit
        self.submitappointor = function (f) {
            var flag = true;
            self.up_error = ko.validation.group(self.UPrice(), { deep: true });
            if (self.up_error().length !== 0) {
                self.up_error.showAllMessages();
                flag = false;
            }
            if (self.UnitHolders().length <= 0) {
                flag = false;
            }
            self.appointorerror = ko.validation.group(self.UnitHolders(), {
                deep: true
            });

            if (self.appointorerror().length !== 0) {
                self.appointorerror.showAllMessages();
                flag = false;
            }
            if (flag) {
                var app = ko.toJS(self.UnitHolders());
                var dt = JSON.stringify({
                    appointer: app,
                    OrdinaryPrice: self.UPrice().UnitPrice(),
                    TotalUnitHolders: self.UPrice().NoUnitHolders()
                });

                $.ajax({
                    type: "POST",
                    beforeSend: function () { self.ajxproc(true); },
                    complete: function () { self.ajxproc(false); },
                    url: "/api/SiteApi/AddTrustAppointer/",
                    data: dt,
                    contentType: "application/json",
                    success: function (d) {
                        if (d.msg == "success") {
                            toastr.success("Trust details added successfully");
                            self.toggletab(4);
                        } else {
                            toastr.error("Error occurred on server, please try again.");
                        }
                    },
                    error: function () {
                        toastr.error("Server error");
                    }
                });
            }
        };
        // Unit holders ends

        //get beneficiaries
        self.getbeneficiaries = function () {
            $.ajax({
                type: "GET",
                beforeSend: function () { self.ajxproc(true); },
                complete: function () { self.ajxproc(false); },
                url: "/api/SiteApi/GetTrustees/",
                contentType: "application/json",
                success: function (d) {
                    if (d.msg == "success") {
                        if (self.trustDetail().TrustType() == "Bare Trust"
                            || self.trustDetail().TrustType() == "Super Fund Trust" || self.trustDetail().TrustType() == "Super Fund Trust - Update"
                        ) {
                            self.Issummary(true);
                        } else {
                            self.Issummary(false);
                            var b = d.data.BeneficiariesMembers;
                            var a = [];
                            for (var i = 0; i < b.length; i++) {
                                var _b = b[i];
                                a.push(new trustees(_b.Id, _b.Name, _b.istrustee));
                            }

                            var c = [];
                            var _c = d.data.BeneficiariesCompany;
                            if (_c.length > 0) {
                                for (var i = 0; i < _c.length; i++) {
                                    var _a = _c[i];
                                    c.push(new beneficiaryCompany(_a.Id, _a.CompanyName, _a.CompanyACN, _a.RegDate, _a.ContactPerson));
                                }
                            } else {
                                c.push(new beneficiaryCompany(0, "", "", new Date(), ""));
                            }

                            self.trusteesDetails(new beneficiarydetails(a, c, d.data.bType));
                            self.validtrustee();
                        }
                        if (self.trustDetail().TrustType() == "Super Fund Trust - Update")
                        {
                            self.setupcost(d.data.Cost.SetupCost+51);
                            self.setupgst(d.data.Cost.SetupGST+5.1);
                            self.totalcost(d.data.Cost.TotalCost+56.10);
                        }
                        else
                        {
                            self.setupcost(d.data.Cost.SetupCost);
                            self.setupgst(d.data.Cost.SetupGST);
                            self.totalcost(d.data.Cost.TotalCost);
                        }
                       
                        self.tabbing(4);
                    } else if (d.msg == "null") {
                        toastr.warning("Please add some members first.");
                    } else {
                        toastr.error("Error occurred on server, please try again.");
                    }
                },
                error: function () {
                    toastr.error("Server error");
                }
            });
        }

        self.validtrustee = function () {
            var isvalid = ko.utils.arrayFirst(self.trusteesDetails().Members(), function (item) {
                if (typeof item.istrustee === 'function') {
                    if (item.istrustee() == true) {
                        return true;
                    } else {
                        return false;
                    }
                } else {
                    return false;
                }
            });

            if (!isvalid) {
                self.terr(true);
            } else {
                self.terr(false);
            }
            return true;
        };

        self.beneficiarysubmit = function (f) {
            var f = true; //error

            if (self.trusteesDetails().bType == "person") {
                if (self.terr()) {
                    f = true;
                } else {
                    f = false;
                }
            } else {
                self.bnfcryerror = ko.validation.group(self.trusteesDetails().Company(), {
                    deep: true
                });
                if (self.bnfcryerror().length !== 0) {
                    self.bnfcryerror.showAllMessages();
                    f = true;
                } else {
                    f = false;
                }
            }

            //console.log(f);
            //console.log(self.trusteesDetails().Company());
            //console.log(self.trusteesDetails().Members());
            //console.log(self.trusteesDetails().bType());

            self.trusteesDetails().Company()[0].RegDate(moment(self.trusteesDetails().Company()[0].RegDate()).format("MM/DD/YYYY"))

            if (!f) {
                var dt = JSON.stringify(ko.toJS(self.trusteesDetails()))
                $.ajax({
                    type: "POST",
                    beforeSend: function () { self.ajxproc(true); },
                    complete: function () { self.ajxproc(false); },
                    url: "/api/SiteApi/updateBeneficiaries/",
                    data: dt,
                    contentType: "application/json",
                    success: function (d) {
                        if (d.msg == "success") {
                            location.href = "/trustsetup/summary";
                        } else if (d.msg = "invalid") {
                            toastr.error("Please select at least 1 Beneficiary");
                        }

                        else {
                            toastr.error("Error occurred on server, please try again.");
                        }
                    },
                    error: function () {
                        toastr.error("Server error");
                    }
                });
            }
        };

        self.toggletab = function (i) {
            var a = parseInt(i);
            if (parseInt(self.tabbing()) != a) {
                switch (a) {
                    case 2:
                        self.fetchTrust();
                        break;
                    case 3:
                        self.fetchTrustAppointer();
                        break;
                    case 4:
                        self.getbeneficiaries();
                        break;
                    default:
                        self.tabbing(a);
                        break;
                }
            }
        };

        self.toggletab(activepnl);
    };

    ko.applyBindings(binndviews(), document.getElementById("bindview"))
};