(function()
{
 var Global=this,Runtime=this.IntelliFactory.Runtime,UI,Next,Doc,List,CentBet,Client,Admin,T,AttrModule,AttrProxy,Key,Var,Concurrency,Var1,Option,Seq,View,RecordType,Strings,Seq1,Remoting,AjaxRemotingProvider,Unchecked,PrintfHelpers,console,Storage1,Json,Provider,Id,ListModel,Work,State,GameDetail,Coupon,PageLen,SettingsDialog,Helpers,Utils,Meetup,View1,Operators,Football,Market,MarketBook,RunnerBook,Option1,Helpers1,LocalStorage,ServerBetfairsSession,Event,Meetup1,Date,JSON,window,Slice,MatchFailureException,Work1,Collections,MapModule;
 Runtime.Define(Global,{
  CentBet:{
   Client:{
    Admin:{
     RecordType:Runtime.Class({},{
      get_color:function()
      {
       return function(_arg1)
       {
        return _arg1.$==1?["lightgrey","red"]:_arg1.$==2?["lightgrey","green"]:["white","navy"];
       };
      }
     }),
     Render:function()
     {
      var arg20;
      arg20=Runtime.New(T,{
       $:0
      });
      return Doc.Concat(List.ofArray([Admin.RenderCommandPrompt(),(Admin.op_SpliceUntyped())(Doc.Element("br",[],arg20)),Admin.RenderRecords()]));
     },
     RenderCommandPrompt:function()
     {
      var arg00;
      arg00=List.ofArray([(Admin.op_SpliceUntyped())(Doc.Element("span",List.ofArray([AttrModule.Style("margin-left","10px")]),Runtime.New(T,{
       $:0
      }))),(Admin.op_SpliceUntyped())(Doc.Element("label",List.ofArray([AttrProxy.Create("for",Admin["cmd-input"]())]),List.ofArray([Doc.TextNode("Input here:")]))),Admin.renderInput()]);
      return Doc.Concat(arg00);
     },
     RenderRecords:function()
     {
      var _arg00_,_arg10_;
      _arg00_=function(r)
      {
       var arg00,arg20;
       arg20=Runtime.New(T,{
        $:0
       });
       arg00=List.ofArray([(Admin.renderRecord())(r),(Admin.op_SpliceUntyped())(Doc.Element("br",[],arg20))]);
       return Doc.Concat(arg00);
      };
      _arg10_=Admin.varConsole().get_View();
      return Doc.ConvertSeq(_arg00_,_arg10_);
     },
     addRecord:function(recordType,text)
     {
      return Admin.varConsole().Add({
       Id:Key.Fresh(),
       Text:text,
       RecordType:recordType
      });
     },
     "cmd-input":Runtime.Field(function()
     {
      return"cmd-input";
     }),
     cmdKey:function(x)
     {
      return x.Id;
     },
     doc:function(x)
     {
      return x;
     },
     op_SpliceUntyped:Runtime.Field(function()
     {
      return function(x)
      {
       return Admin.doc(x);
      };
     }),
     recordKey:function(x)
     {
      return x.Id;
     },
     renderInput:function()
     {
      var varInput,rvFocusInput,varDisableInput,doSend,mapping,setCommandFromHistory,x1,arg00,_arg00_;
      varInput=Var.Create("");
      rvFocusInput=Var.Create(null);
      varDisableInput=Var.Create(false);
      doSend=Concurrency.Delay(function()
      {
       Var1.Set(varDisableInput,true);
       return Concurrency.Bind(Admin.send(Var1.Get(varInput)),function()
       {
        Var1.Set(varDisableInput,false);
        Var1.Set(varInput,"");
        return Concurrency.Return(null);
       });
      });
      mapping=function(cmd)
      {
       var _;
       _={
        $:1,
        $0:cmd
       };
       Admin.varCmd=function()
       {
        return _;
       };
       return Var1.Set(varInput,cmd.Text);
      };
      setCommandFromHistory=function(x)
      {
       var option,value;
       option=Admin.tryGetCommandFromHistory(x);
       value=Option.map(mapping,option);
       return;
      };
      x1=varDisableInput.get_View();
      arg00=function(disable)
      {
       var arg001;
       arg001=List.ofArray([Doc.Input(Seq.toList(Seq.delay(function()
       {
        return Seq.append([AttrProxy.Create("id",Admin["cmd-input"]())],Seq.delay(function()
        {
         return Seq.append(disable?[AttrProxy.Create("disabled","disabled")]:Seq.empty(),Seq.delay(function()
         {
          return Seq.append([AttrModule.Style("width","80%")],Seq.delay(function()
          {
           return Seq.append([AttrModule.CustomVar(rvFocusInput,function(el)
           {
            return function()
            {
             return el.focus();
            };
           },function()
           {
            return{
             $:0
            };
           })],Seq.delay(function()
           {
            return[AttrModule.Handler("keydown",function()
            {
             return function(e)
             {
              var key,_,matchValue;
              key=e.keyCode;
              if(key===13)
               {
                matchValue=Var1.Get(varInput).toLowerCase();
                _=matchValue==="-clear-output"?Admin.varConsole().Clear():matchValue==="-clear-hist"?Admin.varCommandsHistory().Clear():Concurrency.Start(doSend,{
                 $:0
                });
               }
              else
               {
                _=key===38?setCommandFromHistory(true):key===40?setCommandFromHistory(false):null;
               }
              return _;
             };
            })];
           }));
          }));
         }));
        }));
       })),varInput)]);
       return Doc.Concat(arg001);
      };
      _arg00_=View.Map(arg00,x1);
      return Doc.EmbedView(_arg00_);
     },
     renderRecord:Runtime.Field(function()
     {
      var arg00;
      arg00=function(r)
      {
       var _,patternInput,fore,back,e;
       try
       {
        patternInput=(RecordType.get_color())(r.RecordType);
        fore=patternInput[1];
        back=patternInput[0];
        _=(Admin.op_SpliceUntyped())(Doc.Element("span",List.ofArray([AttrModule.Style("color",fore),AttrModule.Style("background",back)]),List.ofArray([Doc.TextNode(r.Text)])));
       }
       catch(e)
       {
        Admin.varCommandsHistory().Clear();
        Admin.varConsole().Clear();
        _=Doc.get_Empty();
       }
       return _;
      };
      return function(x)
      {
       var _arg00_;
       _arg00_=View.Map(arg00,x);
       return Doc.EmbedView(_arg00_);
      };
     }),
     send:function(inputText)
     {
      return Concurrency.Delay(function()
      {
       var inputText1,a,xs,_,x,_1;
       inputText1=Strings.Trim(inputText);
       Admin.addRecord(Runtime.New(RecordType,{
        $:2
       }),inputText1);
       xs=Var1.Get(Admin.varCommandsHistory().Var);
       if(Seq.isEmpty(xs))
        {
         _=true;
        }
       else
        {
         x=Seq1.last(xs);
         _=x.Text!==inputText1;
        }
       if(_)
        {
         Admin.varCommandsHistory().Add({
          Id:Key.Fresh(),
          Text:inputText1
         });
         _1=Concurrency.Return(null);
        }
       else
        {
         _1=Concurrency.Return(null);
        }
       a=_1;
       return Concurrency.Combine(a,Concurrency.Delay(function()
       {
        return Concurrency.TryWith(Concurrency.Delay(function()
        {
         var x1;
         x1=AjaxRemotingProvider.Async("WebFace:2",[inputText1]);
         return Concurrency.Bind(x1,function(_arg1)
         {
          var _2,x2,x3;
          if(_arg1.$==1)
           {
            x2=_arg1.$0;
            Admin.addRecord(Runtime.New(RecordType,{
             $:1
            }),x2);
            _2=Concurrency.Return(null);
           }
          else
           {
            x3=_arg1.$0;
            Admin.addRecord(Runtime.New(RecordType,{
             $:0
            }),x3);
            _2=Concurrency.Return(null);
           }
          return _2;
         });
        }),function(_arg2)
        {
         Admin.addRecord(Runtime.New(RecordType,{
          $:1
         }),_arg2.message);
         return Concurrency.Return(null);
        });
       }));
      });
     },
     tryGetCommandFromHistory:function(isnext)
     {
      var xs,count,_,n,_1,_id_,mapping,predicate,source,source1,v,matchValue,_2,_3,n2,_4,n3,_5,_6,n4,_7,n5,_8,_9,n6,_a,n7,_b,_c,n8,_d,n9,arg0;
      xs=Var1.Get(Admin.varCommandsHistory().Var);
      count=Seq.length(xs);
      if(count===0)
       {
        _={
         $:0
        };
       }
      else
       {
        if(Admin.varCmd().$==1)
         {
          _id_=Admin.varCmd().$0.Id;
          mapping=function(n1)
          {
           return function(x)
           {
            return[x,n1];
           };
          };
          predicate=function(tupledArg)
          {
           var _arg1,_id__;
           _arg1=tupledArg[0];
           tupledArg[1];
           _id__=_arg1.Id;
           return Unchecked.Equals(_id__,_id_);
          };
          source=Var1.Get(Admin.varCommandsHistory().Var);
          source1=Seq.mapi(mapping,source);
          v=Seq.tryFind(predicate,source1);
          matchValue=[v,isnext];
          if(matchValue[0].$==1)
           {
            if(matchValue[1])
             {
              matchValue[0].$0[0];
              n2=matchValue[0].$0[1];
              if(count>0?n2<count-1:false)
               {
                n3=matchValue[0].$0[1];
                matchValue[0].$0[0];
                _4=n3+1;
               }
              else
               {
                if(matchValue[0].$==1)
                 {
                  if(matchValue[1])
                   {
                    _6=matchValue[1]?count-1:0;
                   }
                  else
                   {
                    matchValue[0].$0[0];
                    n4=matchValue[0].$0[1];
                    if(count>0?n4>0:false)
                     {
                      n5=matchValue[0].$0[1];
                      matchValue[0].$0[0];
                      _7=n5-1;
                     }
                    else
                     {
                      _7=matchValue[1]?count-1:0;
                     }
                    _6=_7;
                   }
                  _5=_6;
                 }
                else
                 {
                  _5=matchValue[1]?count-1:0;
                 }
                _4=_5;
               }
              _3=_4;
             }
            else
             {
              if(matchValue[0].$==1)
               {
                if(matchValue[1])
                 {
                  _9=matchValue[1]?count-1:0;
                 }
                else
                 {
                  matchValue[0].$0[0];
                  n6=matchValue[0].$0[1];
                  if(count>0?n6>0:false)
                   {
                    n7=matchValue[0].$0[1];
                    matchValue[0].$0[0];
                    _a=n7-1;
                   }
                  else
                   {
                    _a=matchValue[1]?count-1:0;
                   }
                  _9=_a;
                 }
                _8=_9;
               }
              else
               {
                _8=matchValue[1]?count-1:0;
               }
              _3=_8;
             }
            _2=_3;
           }
          else
           {
            if(matchValue[0].$==1)
             {
              if(matchValue[1])
               {
                _c=matchValue[1]?count-1:0;
               }
              else
               {
                matchValue[0].$0[0];
                n8=matchValue[0].$0[1];
                if(count>0?n8>0:false)
                 {
                  n9=matchValue[0].$0[1];
                  matchValue[0].$0[0];
                  _d=n9-1;
                 }
                else
                 {
                  _d=matchValue[1]?count-1:0;
                 }
                _c=_d;
               }
              _b=_c;
             }
            else
             {
              _b=matchValue[1]?count-1:0;
             }
            _2=_b;
           }
          _1=_2;
         }
        else
         {
          _1=count-1;
         }
        n=_1;
        arg0=Seq.nth(n,xs);
        _={
         $:1,
         $0:arg0
        };
       }
      return _;
     },
     varCmd:Runtime.Field(function()
     {
      return{
       $:0
      };
     }),
     varCommandsHistory:Runtime.Field(function()
     {
      var x,_,clo1,arg00,arg10,x2,clo11,e,clo12,arg001,arg101;
      try
      {
       clo1=function(_1)
       {
        var s;
        s="restoring list - "+PrintfHelpers.prettyPrint(_1);
        return console?console.log(s):undefined;
       };
       clo1("CentBetConsoleCommandsHistory");
       arg00=function(x1)
       {
        return Admin.cmdKey(x1);
       };
       arg10=Storage1.LocalStorage("CentBetConsoleCommandsHistory",{
        Encode:(Provider.get_Default().EncodeRecord(undefined,[["Id",Provider.get_Default().EncodeUnion(Key,"$",[[0,[["$0","Item",Id,0]]]]),0],["Text",Id,0]]))(),
        Decode:(Provider.get_Default().DecodeRecord(undefined,[["Id",Provider.get_Default().DecodeUnion(Key,"$",[[0,[["$0","Item",Id,0]]]]),0],["Text",Id,0]]))()
       });
       x2=ListModel.CreateWithStorage(arg00,arg10);
       clo11=function(_1)
       {
        return function(_2)
        {
         var s;
         s=PrintfHelpers.prettyPrint(_1)+" - "+Global.String(_2);
         return console?console.log(s):undefined;
        };
       };
       (clo11("CentBetConsoleCommandsHistory"))(x2.get_Length());
       _=x2;
      }
      catch(e)
      {
       clo12=function(_1)
       {
        return function(_2)
        {
         var s;
         s="error when restoring "+PrintfHelpers.prettyPrint(_1)+" - "+PrintfHelpers.prettyPrint(_2);
         return console?console.log(s):undefined;
        };
       };
       (clo12("CentBetConsoleCommandsHistory"))(e);
       arg001=function(x1)
       {
        return Admin.cmdKey(x1);
       };
       arg101=Runtime.New(T,{
        $:0
       });
       _=ListModel.Create(arg001,arg101);
      }
      x=_;
      return x;
     }),
     varConsole:Runtime.Field(function()
     {
      var x,_,clo1,arg00,arg10,x2,clo11,e,clo12,arg001,arg101;
      try
      {
       clo1=function(_1)
       {
        var s;
        s="restoring list - "+PrintfHelpers.prettyPrint(_1);
        return console?console.log(s):undefined;
       };
       clo1("CentBetConsole");
       arg00=function(x1)
       {
        return Admin.recordKey(x1);
       };
       arg10=Storage1.LocalStorage("CentBetConsole",{
        Encode:(Provider.get_Default().EncodeRecord(undefined,[["Id",Provider.get_Default().EncodeUnion(Key,"$",[[0,[["$0","Item",Id,0]]]]),0],["Text",Id,0],["RecordType",Provider.get_Default().EncodeUnion(RecordType,"$",[[0,[]],[1,[]],[2,[]]]),0]]))(),
        Decode:(Provider.get_Default().DecodeRecord(undefined,[["Id",Provider.get_Default().DecodeUnion(Key,"$",[[0,[["$0","Item",Id,0]]]]),0],["Text",Id,0],["RecordType",Provider.get_Default().DecodeUnion(RecordType,"$",[[0,[]],[1,[]],[2,[]]]),0]]))()
       });
       x2=ListModel.CreateWithStorage(arg00,arg10);
       clo11=function(_1)
       {
        return function(_2)
        {
         var s;
         s=PrintfHelpers.prettyPrint(_1)+" - "+Global.String(_2);
         return console?console.log(s):undefined;
        };
       };
       (clo11("CentBetConsole"))(x2.get_Length());
       _=x2;
      }
      catch(e)
      {
       clo12=function(_1)
       {
        return function(_2)
        {
         var s;
         s="error when restoring "+PrintfHelpers.prettyPrint(_1)+" - "+PrintfHelpers.prettyPrint(_2);
         return console?console.log(s):undefined;
        };
       };
       (clo12("CentBetConsole"))(e);
       arg001=function(x1)
       {
        return Admin.recordKey(x1);
       };
       arg101=Runtime.New(T,{
        $:0
       });
       _=ListModel.Create(arg001,arg101);
      }
      x=_;
      return x;
     })
    },
    Coupon:{
     Render:function()
     {
      var varDataRecived,arg00,x,arg001,_arg00_1;
      varDataRecived=Var.Create(false);
      Work.start(varDataRecived);
      x=State.varMode().get_View();
      arg001=function(_arg1)
      {
       var _,x1,arg002,arg10,_arg00_;
       if(_arg1.$==1)
        {
         x1=_arg1.$0;
         _=GameDetail.renderExploreMarkets(x1);
        }
       else
        {
         arg002=function(_arg2)
         {
          var _1,x2,arg20,x3;
          if(_arg2)
           {
            x2=Coupon["render\u0421oupon"]();
            _1=Coupon.doc(x2);
           }
          else
           {
            arg20=List.ofArray([Doc.TextNode("\u0414\u0430\u043d\u043d\u044b\u0435 \u0437\u0430\u0433\u0440\u0443\u0436\u0430\u044e\u0442\u0441\u044f \u0441 \u0441\u0435\u0440\u0432\u0435\u0440\u0430. \u041f\u043e\u0436\u0430\u043b\u0443\u0439\u0441\u0442\u0430, \u043f\u043e\u0434\u043e\u0436\u0434\u0438\u0442\u0435.")]);
            x3=Doc.Element("h1",[],arg20);
            _1=Coupon.doc(x3);
           }
          return _1;
         };
         arg10=varDataRecived.get_View();
         _arg00_=View.Map(arg002,arg10);
         _=Doc.EmbedView(_arg00_);
        }
       return _;
      };
      _arg00_1=View.Map(arg001,x);
      arg00=List.ofArray([Doc.EmbedView(_arg00_1)]);
      return Doc.Concat(arg00);
     },
     SettingsDialog:{
      Helpers:{
       buttonDoc:function(n)
       {
        var x,arg00,_arg00_;
        x=PageLen.view();
        arg00=function(pageLen)
        {
         var _,x1;
         if((n?pageLen===40:false)?true:!n?pageLen===10:false)
          {
           _=Doc.get_Empty();
          }
         else
          {
           x1=Helpers.buttonElt(n,pageLen);
           _=Coupon.doc(x1);
          }
         return _;
        };
        _arg00_=View.Map(arg00,x);
        return Doc.EmbedView(_arg00_);
       },
       buttonElt:function(n,pageLen)
       {
        var t;
        t=n?"+":"-";
        return Doc.Element("button",List.ofArray([Helpers.op_SpliceUntyped("w3-btn w3-teal"),AttrProxy.Create("style","margin: 10px; width: 50px; height: 50px;"),AttrModule.Handler("click",function()
        {
         return function()
         {
          var matchValue,_,_1,value,_2,value1;
          matchValue=[n,pageLen];
          if(matchValue[0])
           {
            if(matchValue[1]===40)
             {
              _1=null;
             }
            else
             {
              value=pageLen+(n?1:-1);
              _1=PageLen.set(value);
             }
            _=_1;
           }
          else
           {
            if(matchValue[1]===10)
             {
              _2=null;
             }
            else
             {
              value1=pageLen+(n?1:-1);
              _2=PageLen.set(value1);
             }
            _=_2;
           }
          return _;
         };
        })]),List.ofArray([Doc.TextNode(t)]));
       },
       close:function()
       {
        return Var1.Set(Helpers.varVisible(),false);
       },
       content:Runtime.Field(function()
       {
        var x,x1,arg00,arg10;
        x=Utils.titleAndCloseButton("\u041a\u043e\u043b\u0438\u0447\u0435\u0441\u0442\u0432\u043e \u043c\u0430\u0442\u0447\u0435\u0439 \u043d\u0430 \u0441\u0442\u0440\u0430\u043d\u0438\u0446\u0435",function()
        {
         return Helpers.close();
        });
        arg00=function(value)
        {
         return Global.String(value);
        };
        arg10=PageLen.view();
        x1=Doc.Element("div",List.ofArray([Helpers.op_SpliceUntyped("w3-xxlarge"),AttrProxy.Create("style","margin : 10px; float : left;")]),List.ofArray([Doc.TextView(View.Map(arg00,arg10))]));
        return List.ofArray([Coupon.doc(x),Coupon.doc(x1),Helpers.buttonDoc(true),Helpers.buttonDoc(false)]);
       }),
       op_SpliceUntyped:function(arg00)
       {
        return AttrProxy.Create("class",arg00);
       },
       renderDialog:Runtime.Field(function()
       {
        return Utils.createModal(Helpers.varVisible().get_View(),function()
        {
         return Helpers.close();
        });
       }),
       varVisible:Runtime.Field(function()
       {
        return Var.Create(false);
       })
      },
      elt:Runtime.Field(function()
      {
       return((Helpers.renderDialog())(Runtime.New(T,{
        $:0
       })))(Helpers.content());
      }),
      show:function()
      {
       return Var1.Set(Helpers.varVisible(),true);
      }
     },
     doc:function(x)
     {
      return x;
     },
     renderMeetup:Runtime.Field(function()
     {
      var tupledArg,viewColumnGpbVisible,viewColumnCountryVisible;
      tupledArg=[State.varColumnGpbVisible().get_View(),State.varColumnCountryVisible().get_View()];
      viewColumnGpbVisible=tupledArg[0];
      viewColumnCountryVisible=tupledArg[1];
      return function(x)
      {
       return Meetup.renderMeetup(viewColumnGpbVisible,viewColumnCountryVisible,x);
      };
     }),
     renderPagination:Runtime.Field(function()
     {
      var _builder_,x,_arg00_;
      _builder_=View.get_Do();
      x=State.varPagesCount().get_View();
      _arg00_=View1.Bind(function(_arg1)
      {
       var x1;
       x1=State.varCurrentPageNumber().get_View();
       return View1.Bind(function(_arg2)
       {
        var aShowDialog,list,arg00;
        aShowDialog=AttrModule.Handler("click",function()
        {
         return function()
         {
          return SettingsDialog.show();
         };
        });
        list=Seq.toList(Seq.delay(function()
        {
         return Seq.append(Seq.map(function(n)
         {
          var aattrs,arg20,t;
          aattrs=Seq.toList(Seq.delay(function()
          {
           return Seq.append([AttrProxy.Create("href","#")],Seq.delay(function()
           {
            return Seq.append([AttrModule.Handler("click",function()
            {
             return function()
             {
              return Var1.Set(State.varTargetPageNumber(),n);
             };
            })],Seq.delay(function()
            {
             return n===_arg2?[AttrProxy.Create("class","w3-teal")]:Seq.empty();
            }));
           }));
          }));
          t=Global.String(n+1);
          arg20=List.ofArray([Doc.Element("a",aattrs,List.ofArray([Doc.TextNode(t)]))]);
          return Doc.Element("li",[],arg20);
         },Operators.range(0,_arg1-1)),Seq.delay(function()
         {
          var arg20;
          arg20=List.ofArray([Doc.Element("a",List.ofArray([AttrProxy.Create("href","#"),aShowDialog]),List.ofArray([Doc.TextNode("...")]))]);
          return[Doc.Element("li",[],arg20)];
         }));
        }));
        arg00=List.map(function(x2)
        {
         return Coupon.doc(x2);
        },list);
        return View1.Const(Doc.Concat(arg00));
       },x1);
      },x);
      return Doc.EmbedView(_arg00_);
     }),
     "render\u0421oupon":Runtime.Field(function()
     {
      var ats,arg20,ats1,arg201,arg202,x,f,mapping,arg00,x3,etable,ats2,_builder_,x4,_arg00_;
      ats=List.ofArray([AttrProxy.Create("class","w3-responsive")]);
      ats1=List.ofArray([AttrProxy.Create("class","w3-table w3-bordered w3-striped w3-hoverable")]);
      arg201=List.ofArray([Doc.Element("tr",List.ofArray([AttrModule.Class("coupon-header-row w3-teal")]),Meetup.renderGamesHeaderRow(State.varColumnGpbVisible().get_View()))]);
      x=State.meetups().get_View();
      f=Coupon.renderMeetup();
      mapping=function(x1)
      {
       var x2;
       x2=f(x1);
       return Coupon.doc(x2);
      };
      arg00=function(x1)
      {
       var arg001;
       arg001=Seq.map(mapping,x1);
       return Doc.Concat(arg001);
      };
      x3=View.Map(arg00,x);
      arg202=List.ofArray([Doc.EmbedView(x3)]);
      arg20=List.ofArray([Doc.Element("table",ats1,List.ofArray([Doc.Element("thead",[],arg201),Doc.Element("tbody",[],arg202)]))]);
      etable=Doc.Element("div",ats,arg20);
      ats2=List.ofArray([AttrProxy.Create("class","w3-container")]);
      _builder_=View.get_Do();
      x4=State.varCurrentPageNumber().get_View();
      _arg00_=View1.Bind(function(_arg1)
      {
       return View1.Bind(function(_arg2)
       {
        var x1,_,arg203,t;
        if(_arg1===_arg2)
         {
          _=etable;
         }
        else
         {
          t="\u0412\u044b\u043f\u043e\u043b\u043d\u044f\u0435\u0442\u0441\u044f \u043f\u0435\u0440\u0435\u0445\u043e\u0434 \u043a \u0441\u0442\u0440\u0430\u043d\u0438\u0446\u0435 \u2116 "+Global.String(_arg2+1)+"...";
          arg203=List.ofArray([Doc.TextNode(t)]);
          _=Doc.Element("h1",[],arg203);
         }
        x1=_;
        return View1.Const(Coupon.doc(x1));
       },State.varTargetPageNumber().get_View());
      },x4);
      return Doc.Element("div",ats2,List.ofArray([Doc.Element("div",List.ofArray([AttrProxy.Create("class","w3-center")]),List.ofArray([Doc.Element("ul",List.ofArray([AttrProxy.Create("class","w3-pagination w3-border w3-round")]),List.ofArray([Coupon.renderPagination()]))])),Doc.EmbedView(_arg00_),SettingsDialog.elt()]));
     })
    },
    Football:{
     Event:Runtime.Class({},{
      id:function(x)
      {
       return x.gameId;
      }
     }),
     Market:Runtime.Class({},{
      New:function(marketId,marketName,runners)
      {
       var mapping;
       mapping=function(tupledArg)
       {
        var runnerNamem,selectionId;
        runnerNamem=tupledArg[0];
        selectionId=tupledArg[1];
        return{
         selectionId:selectionId,
         runnerName:runnerNamem
        };
       };
       return Runtime.New(Market,{
        marketId:marketId,
        marketName:marketName,
        runners:List.map(mapping,runners)
       });
      },
      id:function(x)
      {
       return x.marketId;
      }
     }),
     MarketBook:Runtime.Class({},{
      New:function(m)
      {
       var mapping,list;
       mapping=function(arg00)
       {
        return RunnerBook.New(arg00);
       };
       list=m.runners;
       return Runtime.New(MarketBook,{
        marketId:m.marketId,
        marketName:m.marketName,
        visible:Var.Create(false),
        runners:List.map(mapping,list)
       });
      },
      id:function(x)
      {
       return x.marketId;
      }
     }),
     Meetup:Runtime.Class({},{
      id:function(x)
      {
       return x.game.gameId;
      }
     }),
     RunnerBook:Runtime.Class({},{
      New:function(r)
      {
       var x;
       x=function()
       {
        return Var.Create({
         $:0
        });
       };
       return Runtime.New(RunnerBook,{
        selectionId:r.selectionId,
        runnerName:r.runnerName,
        backPrice:x(null),
        backSize:x(null),
        layPrice:x(null),
        laySize:x(null)
       });
      }
     })
    },
    GameDetail:{
     Helpers:{
      renderGameInfo:function(meetup)
      {
       var ats,ats1,ats2,x,def,arg00,x3,ats3,x4,arg001,_arg00_,ats4;
       ats=List.ofArray([GameDetail.op_SpliceUntyped("w3-indigo")]);
       ats1=List.ofArray([GameDetail.op_SpliceUntyped("w3-row")]);
       ats2=List.ofArray([GameDetail.op_SpliceUntyped("w3-col s4"),AttrProxy.Create("style","text-align: right; margin : 10px;")]);
       x=meetup.country.get_View();
       def=Doc.get_Empty();
       arg00=function(x1)
       {
        var x2;
        x2=Option.map(function(t)
        {
         return Doc.TextNode(t);
        },x1);
        return Option1.getWith(def,x2);
       };
       x3=View.Map(arg00,x);
       ats3=List.ofArray([GameDetail.op_SpliceUntyped("w3-col s2"),AttrProxy.Create("style","color : #b4e500; margin : 0 10px;")]);
       x4=meetup.totalMatched.get_View();
       arg001=function(totalMatchs)
       {
        var mapping,folder,source,totalMatchs1;
        mapping=function(tuple)
        {
         return tuple[1];
        };
        folder=function(x1)
        {
         return function(y)
         {
          return x1+y;
         };
        };
        source=List.map(mapping,totalMatchs);
        totalMatchs1=Seq.fold(folder,0,source);
        return totalMatchs1===0?Doc.get_Empty():Doc.TextNode(Global.String(totalMatchs1)+" GPB");
       };
       _arg00_=View.Map(arg001,x4);
       ats4=List.ofArray([GameDetail.op_SpliceUntyped("w3-row")]);
       return Doc.Element("div",ats,List.ofArray([Doc.Element("div",ats1,List.ofArray([Doc.Element("a",List.ofArray([AttrProxy.Create("style","text-align: left; margin-left : 5px;"),GameDetail.op_SpliceUntyped("w3-col s6"),AttrProxy.Create("href","#"),AttrModule.Handler("click",function()
       {
        return function()
        {
         return Var1.Set(State.varMode(),{
          $:0
         });
        };
       })]),List.ofArray([Doc.TextNode("\u041d\u0430\u0437\u0430\u0434 \u043a \u0441\u043f\u0438\u0441\u043a\u0443 \u043c\u0430\u0442\u0447\u0435\u0439")])),Doc.Element("div",ats2,List.ofArray([Doc.EmbedView(x3)])),Doc.Element("div",ats3,List.ofArray([Doc.EmbedView(_arg00_)]))])),Doc.Element("div",ats4,List.ofArray([Doc.Element("h1",List.ofArray([GameDetail.op_SpliceUntyped("w3-col s5"),AttrProxy.Create("style","text-align: right;")]),List.ofArray([Doc.TextNode(meetup.game.home)])),Doc.Element("h1",List.ofArray([GameDetail.op_SpliceUntyped("w3-col s2"),AttrProxy.Create("style","text-align: center; color : #b4e500;")]),List.ofArray([Doc.TextView(meetup.summary.get_View())])),Doc.Element("h1",List.ofArray([GameDetail.op_SpliceUntyped("w3-col s5"),AttrProxy.Create("style","text-align: left;")]),List.ofArray([Doc.TextNode(meetup.game.away)]))])),Doc.Element("div",List.ofArray([AttrProxy.Create("style","width : 100%;")]),List.ofArray([Doc.Element("h2",List.ofArray([AttrProxy.Create("style","text-align: center;")]),List.ofArray([Doc.TextView(meetup.status.get_View())]))]))]));
      },
      renderMarket:function(market)
      {
       var arg20,arg201,arg00,arg10,_arg00_;
       arg00=function(visible)
       {
        return Helpers1.renderMarketTable(market,visible);
       };
       arg10=market.visible.get_View();
       _arg00_=View.Map(arg00,arg10);
       arg201=List.ofArray([Doc.EmbedView(_arg00_)]);
       arg20=List.ofArray([Doc.Element("table",[],arg201)]);
       return Doc.Element("li",[],arg20);
      },
      renderMarketTable:function(market,visible)
      {
       var arg00;
       arg00=Seq.toList(Seq.delay(function()
       {
        var x;
        x=Helpers1.renderRunnersLinkRow(market,visible);
        return Seq.append([GameDetail.doc(x)],Seq.delay(function()
        {
         var _,mapping,list;
         if(visible)
          {
           mapping=function(x1)
           {
            var x2;
            x2=Helpers1.renderRunner(x1);
            return GameDetail.doc(x2);
           };
           list=market.runners;
           _=List.map(mapping,list);
          }
         else
          {
           _=Seq.empty();
          }
         return _;
        }));
       }));
       return Doc.Concat(arg00);
      },
      renderMarkets:function(meetup)
      {
       var a1,mapping,renderMarkets,x2,matchValue,_,_1,_2,_3,xs1,xs2,xs3,ats,x3,xs11,xs21,x4,markets,x5;
       a1=AttrProxy.Create("style","border-right : 1px solid #ddd;");
       mapping=function(x)
       {
        var x1;
        x1=Helpers1.renderMarket(x);
        return GameDetail.doc(x1);
       };
       renderMarkets=function(list)
       {
        return List.map(mapping,list);
       };
       x2=meetup.marketsBook;
       matchValue=(Helpers1.window(3))(x2);
       if(matchValue.$==1)
        {
         if(matchValue.$1.$==1)
          {
           if(matchValue.$1.$1.$==1)
            {
             if(matchValue.$1.$1.$1.$==0)
              {
               xs1=matchValue.$0;
               xs2=matchValue.$1.$0;
               xs3=matchValue.$1.$1.$0;
               ats=List.ofArray([GameDetail.op_SpliceUntyped("w3-row")]);
               x3=Doc.Element("div",ats,List.ofArray([Doc.Element("ul",List.ofArray([GameDetail.op_SpliceUntyped("w3-ul w3-col s4")]),renderMarkets(xs1)),Doc.Element("ul",List.ofArray([GameDetail.op_SpliceUntyped("w3-ul w3-col s4")]),renderMarkets(xs2)),Doc.Element("ul",List.ofArray([GameDetail.op_SpliceUntyped("w3-ul w3-col s4")]),renderMarkets(xs3))]));
               _3=GameDetail.doc(x3);
              }
             else
              {
               _3=Doc.get_Empty();
              }
             _2=_3;
            }
           else
            {
             xs11=matchValue.$0;
             xs21=matchValue.$1.$0;
             x4=Doc.Element("div",List.ofArray([GameDetail.op_SpliceUntyped("w3-row")]),List.ofArray([Doc.Element("ul",List.ofArray([GameDetail.op_SpliceUntyped("w3-ul w3-col s6")]),renderMarkets(xs11)),Doc.Element("ul",List.ofArray([GameDetail.op_SpliceUntyped("w3-ul w3-col s6")]),renderMarkets(xs21))]));
             _2=GameDetail.doc(x4);
            }
           _1=_2;
          }
         else
          {
           markets=matchValue.$0;
           x5=Doc.Element("ul",List.ofArray([GameDetail.op_SpliceUntyped("w3-ul")]),renderMarkets(markets));
           _1=GameDetail.doc(x5);
          }
         _=_1;
        }
       else
        {
         _=Doc.get_Empty();
        }
       return _;
      },
      renderPrice:function(back,ends,v)
      {
       var ats,x,mapping,def,arg00,x3;
       ats=List.ofArray([GameDetail.op_SpliceUntyped("price "+PrintfHelpers.toSafe(back?"back":"lay"))]);
       x=v.get_View();
       mapping=function(x1)
       {
        var x2,t;
        x2=Utils.formatDecimal(x1);
        t=PrintfHelpers.toSafe(x2)+PrintfHelpers.toSafe(ends);
        return Doc.TextNode(t);
       };
       def=Doc.get_Empty();
       arg00=function(x1)
       {
        var x2;
        x2=Option.map(mapping,x1);
        return Option1.getWith(def,x2);
       };
       x3=View.Map(arg00,x);
       return Doc.Element("td",ats,List.ofArray([Doc.EmbedView(x3)]));
      },
      renderRunner:function(r)
      {
       var arg20,arg201;
       arg201=List.ofArray([Doc.TextNode(r.runnerName)]);
       arg20=List.ofArray([Doc.Element("td",[],arg201),Helpers1.renderPrice(true,"",r.backPrice),Helpers1.renderPrice(true,"$",r.backSize),Helpers1.renderPrice(false,"",r.layPrice),Helpers1.renderPrice(false,"$",r.laySize)]);
       return Doc.Element("tr",[],arg20);
      },
      renderRunnersLink:function(market,visible)
      {
       return Doc.Element("a",List.ofArray([AttrProxy.Create("href","#"),AttrModule.Handler("click",function()
       {
        return function()
        {
         return Var1.Set(market.visible,!Var1.Get(market.visible));
        };
       }),GameDetail.op_SpliceUntyped(visible?"w3-teal":"w3-dark-grey"),AttrProxy.Create("style","padding-left : 5px; padding-bottom : 5px; padding-top : 5px;")]),List.ofArray([Doc.TextNode(market.marketName)]));
      },
      renderRunnersLinkRow:function(market,visible)
      {
       var runnersLink,td,_,arg20,arg201;
       runnersLink=Helpers1.renderRunnersLink(market,visible);
       if(visible)
        {
         _=Doc.Element("td",List.ofArray([AttrProxy.Create("colspan","5")]),List.ofArray([runnersLink]));
        }
       else
        {
         arg20=List.ofArray([runnersLink]);
         _=Doc.Element("td",[],arg20);
        }
       td=_;
       arg201=List.ofArray([td]);
       return Doc.Element("tr",[],arg201);
      },
      window:function(n)
      {
       var mapping,projection,mapping1;
       mapping=function(i)
       {
        return function(x)
        {
         return[i%n,x];
        };
       };
       projection=function(tuple)
       {
        return tuple[0];
       };
       mapping1=function(tupledArg)
       {
        var xs,source;
        tupledArg[0];
        xs=tupledArg[1];
        source=Seq.map(function(tuple)
        {
         return tuple[1];
        },xs);
        return Seq.toList(source);
       };
       return function(x)
       {
        var source,source1,source2;
        source=Seq.mapi(mapping,x);
        source1=Seq1.groupBy(projection,source);
        source2=Seq.map(mapping1,source1);
        return Seq.toList(source2);
       };
      }
     },
     doc:function(x)
     {
      return x;
     },
     op_SpliceUntyped:function(arg00)
     {
      return AttrProxy.Create("class",arg00);
     },
     renderExploreMarkets:function(meetup)
     {
      var arg00,x;
      x=Helpers1.renderGameInfo(meetup);
      arg00=List.ofArray([GameDetail.doc(x),Helpers1.renderMarkets(meetup)]);
      return Doc.Concat(arg00);
     },
     varError:Runtime.Field(function()
     {
      return Var.Create(false);
     })
    },
    Meetup:{
     doc:function(x)
     {
      return x;
     },
     renderCountry:function(viewColumnCountryVisible,countryView)
     {
      var _builder_,_arg00_;
      _builder_=View.get_Do();
      _arg00_=View1.Bind(function(_arg1)
      {
       return!_arg1?View1.Const(Doc.get_Empty()):View1.Bind(function(_arg2)
       {
        var x,_,country,arg20;
        if(_arg2.$==1)
         {
          country=_arg2.$0;
          _=Doc.Element("td",List.ofArray([AttrProxy.Create("class","game-country")]),List.ofArray([Doc.TextNode(country)]));
         }
        else
         {
          arg20=Runtime.New(T,{
           $:0
          });
          _=Doc.Element("td",[],arg20);
         }
        x=_;
        return View1.Const(Meetup.doc(x));
       },countryView);
      },viewColumnCountryVisible);
      return Doc.EmbedView(_arg00_);
     },
     renderGameStat:function(x)
     {
      var _builder_,x1,_arg00_;
      _builder_=View.get_Do();
      x1=x.playMinute.get_View();
      _arg00_=View1.Bind(function(_arg1)
      {
       var x2;
       x2=x.summary.get_View();
       return View1.Bind(function(_arg2)
       {
        var _,_1,x3,_2,x4,arg20,x5,_3,x6,arg201,x7;
        if(_arg1.$==1)
         {
          if(_arg2!=="")
           {
            x3=Doc.Element("td",List.ofArray([AttrProxy.Create("class","game-status")]),List.ofArray([Doc.TextNode(_arg2)]));
            _1=Meetup.doc(x3);
           }
          else
           {
            if(_arg2!=="")
             {
              x4=Doc.Element("td",List.ofArray([AttrProxy.Create("class","game-status")]),List.ofArray([Doc.TextNode(_arg2)]));
              _2=Meetup.doc(x4);
             }
            else
             {
              arg20=Runtime.New(T,{
               $:0
              });
              x5=Doc.Element("td",[],arg20);
              _2=Meetup.doc(x5);
             }
            _1=_2;
           }
          _=_1;
         }
        else
         {
          if(_arg2!=="")
           {
            x6=Doc.Element("td",List.ofArray([AttrProxy.Create("class","game-status")]),List.ofArray([Doc.TextNode(_arg2)]));
            _3=Meetup.doc(x6);
           }
          else
           {
            arg201=Runtime.New(T,{
             $:0
            });
            x7=Doc.Element("td",[],arg201);
            _3=Meetup.doc(x7);
           }
          _=_3;
         }
        return View1.Const(_);
       },x2);
      },x1);
      return Doc.EmbedView(_arg00_);
     },
     renderGamesHeaderRow:function(viewColumnGpbVisible)
     {
      return Seq.toList(Seq.delay(function()
      {
       var list,arg20,arg201,arg202,arg203,arg204;
       arg20=List.ofArray([Doc.TextNode("\u2116")]);
       arg201=List.ofArray([Doc.TextNode("\u0414\u043e\u043c\u0430")]);
       arg202=Runtime.New(T,{
        $:0
       });
       arg203=List.ofArray([Doc.TextNode("\u0412 \u0433\u043e\u0441\u0442\u044f\u0445")]);
       arg204=Runtime.New(T,{
        $:0
       });
       list=List.ofArray([Doc.Element("th",[],arg20),Doc.Element("th",[],arg201),Doc.Element("th",[],arg202),Doc.Element("th",[],arg203),Doc.Element("th",[],arg204),Doc.Element("th",List.ofArray([AttrProxy.Create("colspan","2")]),List.ofArray([Doc.TextNode("\u041f\u043e\u0431\u0435\u0434\u0430")])),Doc.Element("th",List.ofArray([AttrProxy.Create("colspan","2")]),List.ofArray([Doc.TextNode("\u041d\u0438\u0447\u044c\u044f")])),Doc.Element("th",List.ofArray([AttrProxy.Create("colspan","2")]),List.ofArray([Doc.TextNode("\u041f\u043e\u0440\u0430\u0436\u0435\u043d\u0438\u0435")]))]);
       return Seq.append(List.map(function(x)
       {
        return Meetup.doc(x);
       },list),Seq.delay(function()
       {
        var arg00,_arg00_;
        arg00=function(_arg1)
        {
         var _,arg205,x;
         if(_arg1)
          {
           arg205=List.ofArray([Doc.TextNode("GPB")]);
           x=Doc.Element("th",[],arg205);
           _=Meetup.doc(x);
          }
         else
          {
           _=Doc.get_Empty();
          }
         return _;
        };
        _arg00_=View.Map(arg00,viewColumnGpbVisible);
        return Seq.append([Doc.EmbedView(_arg00_)],Seq.delay(function()
        {
         var arg205,x;
         arg205=Runtime.New(T,{
          $:0
         });
         x=Doc.Element("th",[],arg205);
         return[Meetup.doc(x)];
        }));
       }));
      }));
     },
     renderKef:function(back,sel,v)
     {
      var _,arg00,arg001,arg10,v1;
      _=back?"back":"lay";
      arg00="kef "+PrintfHelpers.toSafe(sel)+" "+PrintfHelpers.toSafe(_);
      arg001=function(_arg1)
      {
       return Utils.formatDecimalOption(_arg1);
      };
      arg10=v.get_View();
      v1=View.Map(arg001,arg10);
      return Doc.Element("td",List.ofArray([AttrProxy.Create("class",arg00)]),List.ofArray([Doc.TextView(v1)]));
     },
     renderMeetup:function(viewColumnGpbVisible,viewColumnCountryVisible,x)
     {
      var _bck_,_lay_,showGameDetailOnClick,renderMarketsLink,ch,arg20,arg00,arg10,v1,x2,x3,x4,x5,x6,x7,x8,x9,xa,xb;
      _bck_=function(sel)
      {
       return function(v)
       {
        return Meetup.renderKef(true,sel,v);
       };
      };
      _lay_=function(sel)
      {
       return function(v)
       {
        return Meetup.renderKef(false,sel,v);
       };
      };
      showGameDetailOnClick=AttrModule.Handler("click",function()
      {
       return function()
       {
        return Var1.Set(State.varMode(),{
         $:1,
         $0:x
        });
       };
      });
      renderMarketsLink=function(_text_)
      {
       var x1;
       x1=Doc.Element("a",List.ofArray([AttrProxy.Create("href","#"),showGameDetailOnClick]),List.ofArray([Doc.TextNode(_text_)]));
       return Meetup.doc(x1);
      };
      arg00=function(tupledArg)
      {
       var page,n;
       page=tupledArg[0];
       n=tupledArg[1];
       return Global.String(page)+"."+Global.String(n);
      };
      arg10=x.order.get_View();
      v1=View.Map(arg00,arg10);
      arg20=List.ofArray([Doc.TextView(v1)]);
      x2=Doc.Element("td",[],arg20);
      x3=Doc.Element("td",List.ofArray([AttrProxy.Create("class","home-team")]),List.ofArray([renderMarketsLink(x.game.home)]));
      x4=Doc.Element("td",List.ofArray([AttrProxy.Create("class","away-team")]),List.ofArray([renderMarketsLink(x.game.away)]));
      x5=Doc.Element("td",List.ofArray([AttrProxy.Create("class","game-status")]),List.ofArray([Doc.TextView(x.status.get_View())]));
      x6=(_bck_("win"))(x.winBack);
      x7=(_lay_("win"))(x.winLay);
      x8=(_bck_("draw"))(x.drawBack);
      x9=(_lay_("draw"))(x.drawLay);
      xa=(_bck_("lose"))(x.loseBack);
      xb=(_lay_("lose"))(x.loseLay);
      ch=List.ofArray([Meetup.doc(x2),Meetup.doc(x3),Meetup.renderGameStat(x),Meetup.doc(x4),Meetup.doc(x5),Meetup.doc(x6),Meetup.doc(x7),Meetup.doc(x8),Meetup.doc(x9),Meetup.doc(xa),Meetup.doc(xb),Meetup.renderTotalMatched(viewColumnGpbVisible,x),Meetup.renderCountry(viewColumnCountryVisible,x.country.get_View())]);
      return Doc.Element("tr",[],ch);
     },
     renderTotalMatched:function(viewColumnGpbVisible,x)
     {
      var _builder_,_arg00_;
      _builder_=View.get_Do();
      _arg00_=View1.Bind(function(_arg1)
      {
       var _,x1;
       if(!_arg1)
        {
         _=View1.Const(Doc.get_Empty());
        }
       else
        {
         x1=x.totalMatched.get_View();
         _=View1.Bind(function(_arg2)
         {
          var mapping,folder,source,totalMatchs,x3,_1,arg20,t;
          mapping=function(tuple)
          {
           return tuple[1];
          };
          folder=function(x2)
          {
           return function(y)
           {
            return x2+y;
           };
          };
          source=List.map(mapping,_arg2);
          totalMatchs=Seq.fold(folder,0,source);
          if(totalMatchs===0)
           {
            arg20=Runtime.New(T,{
             $:0
            });
            _1=Doc.Element("td",[],arg20);
           }
          else
           {
            t=Global.String(totalMatchs);
            _1=Doc.Element("td",List.ofArray([AttrProxy.Create("class","game-gpb")]),List.ofArray([Doc.TextNode(t)]));
           }
          x3=_1;
          return View1.Const(Meetup.doc(x3));
         },x1);
        }
       return _;
      },viewColumnGpbVisible);
      return Doc.EmbedView(_arg00_);
     }
    },
    State:{
     PageLen:{
      get:function()
      {
       return PageLen.validateValue(Var1.Get(PageLen["var"]()));
      },
      localStorageKey:Runtime.Field(function()
      {
       return"pageLen";
      }),
      set:function(value)
      {
       var value1,_;
       value1=PageLen.validateValue(value);
       if(value1!==Var1.Get(PageLen["var"]()))
        {
         Var1.Set(PageLen["var"](),value1);
         _=LocalStorage.set(PageLen.localStorageKey(),value1);
        }
       else
        {
         _=null;
        }
       return _;
      },
      validateValue:function(v)
      {
       return v<10?30:v>40?40:v;
      },
      "var":Runtime.Field(function()
      {
       var v,arg00;
       v=LocalStorage.getWithDef(PageLen.localStorageKey(),30);
       arg00=PageLen.validateValue(v);
       return Var.Create(arg00);
      }),
      view:Runtime.Field(function()
      {
       return PageLen["var"]().get_View();
      })
     },
     ServerBetfairsSession:{
      check:Runtime.Field(function()
      {
       return Concurrency.Delay(function()
       {
        return Concurrency.Bind(AjaxRemotingProvider.Async("WebFace:3",[]),function(_arg1)
        {
         ServerBetfairsSession.hasServerBetfairsSession=function()
         {
          return _arg1;
         };
         return Concurrency.Return(null);
        });
       });
      }),
      has:function()
      {
       return ServerBetfairsSession.hasServerBetfairsSession();
      },
      hasNot:function()
      {
       return!ServerBetfairsSession.hasServerBetfairsSession();
      },
      hasServerBetfairsSession:Runtime.Field(function()
      {
       return true;
      })
     },
     events:Runtime.Field(function()
     {
      var dt,x,_,arg00,arg10,enc,e,clo1,arg002,arg101,clo11;
      dt=LocalStorage.checkTodayKey("CentBetEventsCatalogueCreated","CentBetEventsCatalogue");
      try
      {
       arg00=function(arg001)
       {
        return Event.id(arg001);
       };
       enc=(Provider.get_Default().EncodeRecord(Event,[["gameId",Provider.get_Default().EncodeTuple([Id,Id]),0],["country",Id,1],["markets",Provider.get_Default().EncodeList(Provider.get_Default().EncodeRecord(Market,[["marketId",Id,0],["marketName",Id,0],["runners",Provider.get_Default().EncodeList(Provider.get_Default().EncodeRecord(undefined,[["selectionId",Id,0],["runnerName",Id,0]])),0]])),0]]))();
       arg10=Storage1.LocalStorage("CentBetEventsCatalogue",{
        Encode:enc,
        Decode:(Provider.get_Default().DecodeRecord(Event,[["gameId",Provider.get_Default().DecodeTuple([Id,Id]),0],["country",Id,1],["markets",Provider.get_Default().DecodeList(Provider.get_Default().DecodeRecord(Market,[["marketId",Id,0],["marketName",Id,0],["runners",Provider.get_Default().DecodeList(Provider.get_Default().DecodeRecord(undefined,[["selectionId",Id,0],["runnerName",Id,0]])),0]])),0]]))()
       });
       _=ListModel.CreateWithStorage(arg00,arg10);
      }
      catch(e)
      {
       clo1=function(_1)
       {
        return function(_2)
        {
         var s;
         s="error when restoring "+PrintfHelpers.prettyPrint(_1)+" - "+PrintfHelpers.prettyPrint(_2);
         return console?console.log(s):undefined;
        };
       };
       (clo1("CentBetEventsCatalogue"))(e);
       arg002=function(arg001)
       {
        return Event.id(arg001);
       };
       arg101=Runtime.New(T,{
        $:0
       });
       _=ListModel.Create(arg002,arg101);
      }
      x=_;
      clo11=function(_1)
      {
       return function(_2)
       {
        return function(_3)
        {
         var s;
         s=PrintfHelpers.prettyPrint(_1)+" - "+Global.String(_2)+", "+PrintfHelpers.prettyPrint(_3);
         return console?console.log(s):undefined;
        };
       };
      };
      ((clo11("CentBetEventsCatalogue"))(x.get_Length()))(dt);
      return x;
     }),
     isCouponMode:function()
     {
      return Unchecked.Equals(Var1.Get(State.varMode()),{
       $:0
      });
     },
     meetups:Runtime.Field(function()
     {
      var arg00,arg10;
      arg00=function(arg001)
      {
       return Meetup1.id(arg001);
      };
      arg10=Runtime.New(T,{
       $:0
      });
      return ListModel.Create(arg00,arg10);
     }),
     mtpevt:Runtime.Field(function()
     {
      return function(x)
      {
       var tupledArg,_arg00_,_arg01_;
       tupledArg=Meetup1.id(x);
       _arg00_=tupledArg[0];
       _arg01_=tupledArg[1];
       return State.tryGetEventById(_arg00_,_arg01_);
      };
     }),
     notCouponMode:function()
     {
      var value;
      value=State.isCouponMode();
      return!value;
     },
     tryGetCountry:Runtime.Field(function()
     {
      var binder;
      binder=function(e)
      {
       return e.country;
      };
      return function(x)
      {
       var _arg00_,_arg01_,option;
       _arg00_=x[0];
       _arg01_=x[1];
       option=State.tryGetEventById(_arg00_,_arg01_);
       return Option.bind(binder,option);
      };
     }),
     tryGetEventById:function(_,_1)
     {
      var gameId,predicate,source;
      gameId=[_,_1];
      predicate=function(_arg1)
      {
       var _gameId_;
       _gameId_=_arg1.gameId;
       return Unchecked.Equals(gameId,_gameId_);
      };
      source=Var1.Get(State.events().Var);
      return Seq.tryFind(predicate,source);
     },
     tryGetMarkets:Runtime.Field(function()
     {
      var mapping;
      mapping=function(e)
      {
       return e.markets;
      };
      return function(x)
      {
       var _arg00_,_arg01_,option;
       _arg00_=x[0];
       _arg01_=x[1];
       option=State.tryGetEventById(_arg00_,_arg01_);
       return Option.map(mapping,option);
      };
     }),
     varColumnCountryVisible:Runtime.Field(function()
     {
      return Var.Create(false);
     }),
     varColumnGpbVisible:Runtime.Field(function()
     {
      return Var.Create(false);
     }),
     varCurrentPageNumber:Runtime.Field(function()
     {
      return Var.Create(0);
     }),
     varGameDetailError:Runtime.Field(function()
     {
      return Var.Create(false);
     }),
     varMode:Runtime.Field(function()
     {
      return Var.Create({
       $:0
      });
     }),
     varPagesCount:Runtime.Field(function()
     {
      return Var.Create(1);
     }),
     varTargetPageNumber:Runtime.Field(function()
     {
      return Var.Create(0);
     })
    },
    Utils:{
     Err:function(arg0)
     {
      return{
       $:0,
       $0:arg0
      };
     },
     LocalStorage:{
      checkTodayKey:function(createdKey,key)
      {
       var now,creationDate,_,clo1;
       now=Date.now();
       creationDate=LocalStorage.getWithDef(createdKey,now);
       if(now-creationDate>1*86400000)
        {
         LocalStorage.clear(key);
         LocalStorage.set(createdKey,Date.now());
         clo1=function(_1)
         {
          return function(_2)
          {
           var s;
           s="local storage "+PrintfHelpers.prettyPrint(_1)+" of "+PrintfHelpers.prettyPrint(_2)+" is inspired";
           return console?console.log(s):undefined;
          };
         };
         (clo1(key))(creationDate);
         _=now;
        }
       else
        {
         _=creationDate;
        }
       return _;
      },
      clear:function(k)
      {
       return LocalStorage.strg().removeItem(k);
      },
      get:function(k)
      {
       var _,arg00,value,e,clo1;
       try
       {
        arg00=LocalStorage.strg().getItem(k);
        value=JSON.parse(arg00);
        _={
         $:1,
         $0:value
        };
       }
       catch(e)
       {
        clo1=function(_1)
        {
         return function(_2)
         {
          var s;
          s="error getting from local storage, key "+PrintfHelpers.prettyPrint(_1)+" : "+PrintfHelpers.prettyPrint(_2);
          return console?console.log(s):undefined;
         };
        };
        (clo1(k))(e);
        _={
         $:0
        };
       }
       return _;
      },
      getWithDef:function(k,def)
      {
       var matchValue,_,k1;
       matchValue=LocalStorage.get(k);
       if(matchValue.$==1)
        {
         k1=matchValue.$0;
         _=k1;
        }
       else
        {
         _=def;
        }
       return _;
      },
      set:function(k,value)
      {
       var _,value1,error,clo1;
       try
       {
        value1=JSON.stringify(value);
        _=LocalStorage.strg().setItem(k,value1);
       }
       catch(error)
       {
        clo1=function(_1)
        {
         return function(_2)
         {
          return function(_3)
          {
           return Operators.FailWith("error setting to local storage, key "+PrintfHelpers.prettyPrint(_1)+", value "+PrintfHelpers.prettyPrint(_2)+" : "+PrintfHelpers.prettyPrint(_3));
          };
         };
        };
        _=((clo1(k))(value))(error);
       }
       return _;
      },
      strg:Runtime.Field(function()
      {
       return window.localStorage;
      })
     },
     Ok:function(arg0)
     {
      return{
       $:1,
       $0:arg0
      };
     },
     Option:{
      getWith:function(def,x)
      {
       var _,_x_;
       if(x.$==0)
        {
         _=def;
        }
       else
        {
         _x_=x.$0;
         _=_x_;
        }
       return _;
      }
     },
     createModal:function(viewVisible,close)
     {
      var doc,op_SpliceUntyped,render;
      doc=function(x)
      {
       return x;
      };
      op_SpliceUntyped=function(arg00)
      {
       return AttrProxy.Create("class",arg00);
      };
      (Utils.initializeModalVar())(close);
      render=function(attrs)
      {
       return function(content)
       {
        var arg00,_arg00_;
        arg00=function(_arg1)
        {
         return _arg1?doc(Doc.Element("div",List.ofArray([op_SpliceUntyped("w3-modal"),AttrProxy.Create("style","display : block;")]),List.ofArray([Doc.Element("div",Seq.toList(Seq.delay(function()
         {
          return Seq.append(attrs,Seq.delay(function()
          {
           return[op_SpliceUntyped("w3-modal-content w3-animate-zoom w3-card-8")];
          }));
         })),content)]))):Doc.get_Empty();
        };
        _arg00_=View.Map(arg00,viewVisible);
        return Doc.EmbedView(_arg00_);
       };
      };
      return render;
     },
     dateTimeToString:function($s)
     {
      var $0=this,$this=this;
      return Global.dateTimeToString($s);
     },
     formatDecimal:function(x)
     {
      return Utils.trimEnd(x.toFixed(6),List.ofArray([48,46]));
     },
     formatDecimalOption:function(_arg1)
     {
      var _,x;
      if(_arg1.$==1)
       {
        x=_arg1.$0;
        _=Utils.formatDecimal(x);
       }
      else
       {
        _="";
       }
      return _;
     },
     getDatePart:function(x)
     {
      var y;
      y=new Date(x.getTime());
      y.setHours(0,0,0,0);
      return y;
     },
     initializeModalVar:Runtime.Field(function()
     {
      var hs,onclick;
      hs=[Runtime.New(T,{
       $:0
      })];
      onclick=function(e)
      {
       var _,action,list;
       if(e.target.className==="w3-modal")
        {
         action=function(h)
         {
          return h(null);
         };
         list=hs[0];
         _=Seq.iter(action,list);
        }
       else
        {
         _=null;
        }
       return _;
      };
      window.addEventListener("click",onclick,false);
      return function(h)
      {
       hs[0]=Runtime.New(T,{
        $:1,
        $0:h,
        $1:hs[0]
       });
      };
     }),
     titleAndCloseButton:function(title,close)
     {
      var op_SpliceUntyped,ats;
      op_SpliceUntyped=function(arg00)
      {
       return AttrProxy.Create("class",arg00);
      };
      ats=List.ofArray([op_SpliceUntyped("w3-row w3-teal")]);
      return Doc.Element("header",ats,List.ofArray([Doc.Element("h4",List.ofArray([op_SpliceUntyped("w3-col s11 w3-center"),AttrProxy.Create("style","padding-left : 10px;")]),List.ofArray([Doc.TextNode(title)])),Doc.Element("div",List.ofArray([op_SpliceUntyped("w3-col s1 w3-center")]),List.ofArray([Doc.Element("span",List.ofArray([op_SpliceUntyped("w3-closebtn"),AttrProxy.Create("style","padding-right : 5px;"),AttrModule.Handler("click",function()
      {
       return function()
       {
        return close(null);
       };
      })]),List.ofArray([Doc.TextNode("×")]))]))]));
     },
     trimEnd:function(_,_1)
     {
      var _arg1,_2,_3,s,c,_4,rx,s1,_5,s2,rx1,s3;
      _arg1=[_,_1];
      if(_arg1[0]==="")
       {
        _2="";
       }
      else
       {
        if(_arg1[1].$==1)
         {
          s=_arg1[0];
          _arg1[1];
          c=_arg1[1].$0;
          if(s.charCodeAt(s.length-1)===c)
           {
            _arg1[1].$0;
            rx=_arg1[1];
            s1=_arg1[0];
            _4=Utils.trimEnd(Slice.string(s1,{
             $:0
            },{
             $:1,
             $0:s1.length-2
            }),rx);
           }
          else
           {
            if(_arg1[1].$==1)
             {
              s2=_arg1[0];
              rx1=_arg1[1].$1;
              _5=Utils.trimEnd(s2,rx1);
             }
            else
             {
              _5=Operators.Raise(MatchFailureException.New("C:\\Users\\fpawel\\Documents\\Visual Studio 2015\\Projects\\Betfair\\CentBet\\WebFace\\ClientUtils.fs",8,18));
             }
            _4=_5;
           }
          _3=_4;
         }
        else
         {
          s3=_arg1[0];
          _3=s3;
         }
        _2=_3;
       }
      return _2;
     },
     updateVarValue:function(x,value)
     {
      return!Unchecked.Equals(Var1.Get(x),value)?Var1.Set(x,value):null;
     },
     "|Err|Ok|":function(_arg1)
     {
      var _,b,a;
      if(_arg1.$==1)
       {
        b=_arg1.$0;
        _={
         $:1,
         $0:b
        };
       }
      else
       {
        a=_arg1.$0;
        _={
         $:0,
         $0:a
        };
       }
      return _;
     }
    },
    Work:{
     Work:Runtime.Class({},{
      loop:function(x)
      {
       return Concurrency.Delay(function()
       {
        var a;
        a=Concurrency.Delay(function()
        {
         return Concurrency.Bind(x.work,function()
         {
          return Concurrency.Bind(Concurrency.Sleep(x.sleepInterval),function()
          {
           return Concurrency.Return(null);
          });
         });
        });
        return Concurrency.Combine(Concurrency.TryWith(a,function(_arg3)
        {
         var clo1,arg10;
         clo1=function(_)
         {
          return function(_1)
          {
           var s;
           s="task error "+PrintfHelpers.prettyPrint(_)+" : "+PrintfHelpers.prettyPrint(_1);
           return console?console.log(s):undefined;
          };
         };
         arg10=x.what;
         (clo1(arg10))(_arg3);
         return Concurrency.Bind(Concurrency.Sleep(x.sleepErrorInterval),function()
         {
          return Concurrency.Return(null);
         });
        }),Concurrency.Delay(function()
        {
         return Work1.loop(x);
        }));
       });
      },
      "new":function(what,sleepInterval,sleepErrorInterval,work)
      {
       var arg00;
       arg00=Runtime.New(Work1,{
        what:what,
        sleepInterval:sleepInterval,
        sleepErrorInterval:sleepErrorInterval,
        work:work
       });
       return Work1.run(arg00);
      },
      run:function(x)
      {
       var arg00;
       arg00=Concurrency.Delay(function()
       {
        var clo1,arg10;
        clo1=function(_)
        {
         var s;
         s="task "+PrintfHelpers.prettyPrint(_)+" : started";
         return console?console.log(s):undefined;
        };
        arg10=x.what;
        clo1(arg10);
        return Concurrency.Bind(Work1.loop(x),function()
        {
         var clo11,arg101;
         clo11=function(_)
         {
          var s;
          s="task "+PrintfHelpers.prettyPrint(_)+" : terminated";
          return console?console.log(s):undefined;
         };
         arg101=x.what;
         clo11(arg101);
         return Concurrency.Return(null);
        });
       });
       return Concurrency.Start(arg00,{
        $:0
       });
      }
     }),
     addNewGames:function(newGames)
     {
      var source,existedMeetups,mapping,projection,action,source2,source1,source3;
      source=Var1.Get(State.meetups().Var);
      existedMeetups=Seq.toList(source);
      State.meetups().Clear();
      mapping=function(tupledArg)
      {
       var game,i,hash,tupledArg1,_arg00_,_arg01_,mapping1,option,x,def,marketsBook;
       game=tupledArg[0];
       i=tupledArg[1];
       hash=tupledArg[2];
       tupledArg1=game.gameId;
       _arg00_=tupledArg1[0];
       _arg01_=tupledArg1[1];
       mapping1=function(e)
       {
        return List.map(function(arg00)
        {
         return MarketBook.New(arg00);
        },e.markets);
       };
       option=State.tryGetEventById(_arg00_,_arg01_);
       x=Option.map(mapping1,option);
       def=Runtime.New(T,{
        $:0
       });
       marketsBook=Option1.getWith(def,x);
       return Runtime.New(Meetup1,{
        game:game,
        playMinute:Var.Create(i.playMinute),
        status:Var.Create(i.status),
        summary:Var.Create(i.summary),
        order:Var.Create(i.order),
        winBack:Var.Create(i.winBack),
        winLay:Var.Create(i.winLay),
        drawBack:Var.Create(i.drawBack),
        drawLay:Var.Create(i.drawLay),
        loseBack:Var.Create(i.loseBack),
        loseLay:Var.Create(i.loseLay),
        marketsBook:marketsBook,
        totalMatched:Var.Create(Runtime.New(T,{
         $:0
        })),
        country:Var.Create((State.tryGetCountry())(game.gameId)),
        hash:hash
       });
      };
      projection=function(x)
      {
       return Var1.Get(x.order);
      };
      action=function(arg00)
      {
       return State.meetups().Add(arg00);
      };
      source2=List.map(mapping,newGames);
      source1=Seq.append(existedMeetups,source2);
      source3=Seq.sortBy(projection,source1);
      return Seq.iter(action,source3);
     },
     processCoupon:function(varDataRecived)
     {
      return Concurrency.Delay(function()
      {
       var _,mapping,source,source1,request,pagelen,x;
       if(State.notCouponMode())
        {
         _=Concurrency.Return(null);
        }
       else
        {
         mapping=function(m)
         {
          return[m.game.gameId,m.hash];
         };
         source=Var1.Get(State.meetups().Var);
         source1=Seq.map(mapping,source);
         request=Seq.toList(source1);
         pagelen=PageLen.get();
         x=AjaxRemotingProvider.Async("WebFace:0",[request,Var1.Get(State.varTargetPageNumber()),pagelen]);
         _=Concurrency.Bind(x,function(_arg1)
         {
          var updGms,outGms,newGms,gamesCount,pagesCount,a,_1;
          updGms=_arg1[1];
          outGms=_arg1[2];
          newGms=_arg1[0];
          gamesCount=_arg1[3];
          pagesCount=(gamesCount/pagelen>>0)+(gamesCount%pagelen===0?0:1);
          Utils.updateVarValue(State.varPagesCount(),pagesCount);
          if(Var1.Get(State.varTargetPageNumber())>pagesCount)
           {
            Var1.Set(State.varTargetPageNumber(),0);
            _1=Concurrency.Return(null);
           }
          else
           {
            _1=Concurrency.Return(null);
           }
          a=_1;
          return Concurrency.Combine(a,Concurrency.Delay(function()
          {
           var a1,value,_2;
           value=newGms.$==0;
           if(!value)
            {
             Work.addNewGames(newGms);
             _2=Concurrency.Return(null);
            }
           else
            {
             _2=Concurrency.Return(null);
            }
           a1=_2;
           return Concurrency.Combine(a1,Concurrency.Delay(function()
           {
            var action;
            action=function(arg00)
            {
             return State.meetups().RemoveByKey(arg00);
            };
            Seq.iter(action,outGms);
            (Work.updateCurentCoupon())(updGms);
            Work.updateColumnCountryVisible();
            Work.updateColumnGpbVisible();
            Utils.updateVarValue(varDataRecived,true);
            Utils.updateVarValue(State.varCurrentPageNumber(),Var1.Get(State.varTargetPageNumber()));
            return Concurrency.Return(null);
           }));
          }));
         });
        }
       return _;
      });
     },
     processEvents:Runtime.Field(function()
     {
      return Concurrency.Delay(function()
      {
       var _,x,f,predicate,gamesWithoutEvent,_1,mapping,source,ids,x2;
       if(State.notCouponMode())
        {
         _=Concurrency.Return(null);
        }
       else
        {
         x=Var1.Get(State.meetups().Var);
         f=State.mtpevt();
         predicate=function(x1)
         {
          var option;
          option=f(x1);
          return option.$==0;
         };
         gamesWithoutEvent=Seq.filter(predicate,x);
         if(Seq.isEmpty(gamesWithoutEvent)?true:ServerBetfairsSession.hasNot())
          {
           _1=Concurrency.Return(null);
          }
         else
          {
           mapping=function(m)
           {
            return m.game.gameId;
           };
           source=Seq.map(mapping,gamesWithoutEvent);
           ids=Seq.toList(source);
           x2=AjaxRemotingProvider.Async("WebFace:4",[ids]);
           _1=Concurrency.Bind(x2,function(_arg1)
           {
            return Concurrency.Combine(Concurrency.For(_arg1,function(_arg2)
            {
             var name,gameId,country,evt,action,option;
             name=_arg2[1];
             gameId=_arg2[0];
             country=_arg2[2];
             evt=Runtime.New(Event,{
              gameId:gameId,
              country:country,
              markets:Runtime.New(T,{
               $:0
              })
             });
             State.events().Add(evt);
             action=function(m)
             {
              return Var1.Set(m.country,evt.country);
             };
             option=State.meetups().TryFindByKey(gameId);
             Option.iter(action,option);
             return Concurrency.Return(null);
            }),Concurrency.Delay(function()
            {
             Work.updateColumnCountryVisible();
             Work.updateColumnGpbVisible();
             return Concurrency.Return(null);
            }));
           });
          }
         _=_1;
        }
       return _;
      });
     }),
     processGameDetail:Runtime.Field(function()
     {
      return Concurrency.Delay(function()
      {
       var matchValue,_,mtp,tupledArg,_arg00_,_arg01_,x;
       matchValue=Var1.Get(State.varMode());
       if(matchValue.$==1)
        {
         mtp=matchValue.$0;
         tupledArg=mtp.game.gameId;
         _arg00_=tupledArg[0];
         _arg01_=tupledArg[1];
         x=AjaxRemotingProvider.Async("WebFace:1",[_arg00_,_arg01_]);
         _=Concurrency.Bind(x,function(_arg1)
         {
          var arg00,arg10,a,_1,i,h;
          arg00=State.varGameDetailError();
          arg10=_arg1.$==0;
          Var1.Set(arg00,arg10);
          if(_arg1.$==1)
           {
            i=_arg1.$0[0];
            h=_arg1.$0[1];
            Work.updateGameInfo(mtp,i,h);
            _1=Concurrency.Return(null);
           }
          else
           {
            _1=Concurrency.Return(null);
           }
          a=_1;
          return Concurrency.Combine(a,Concurrency.Delay(function()
          {
           var markets,mapping,source,marketsIds,x1;
           markets=mtp.marketsBook;
           mapping=function(arg001)
           {
            return MarketBook.id(arg001);
           };
           source=Seq.map(mapping,markets);
           marketsIds=Seq.toList(source);
           x1=AjaxRemotingProvider.Async("WebFace:7",[marketsIds]);
           return Concurrency.Bind(x1,function(_arg2)
           {
            var action;
            action=function(market)
            {
             var x2,action1;
             x2=market.runners;
             action1=function(runner)
             {
              var op_EqualsEqualsGreater,key,x4,action2;
              op_EqualsEqualsGreater=function(v,x3)
              {
               return Utils.updateVarValue(v,MapModule.TryFind(runner.selectionId,x3));
              };
              key=market.marketId;
              x4=MapModule.TryFind(key,_arg2);
              action2=function(tupledArg1)
              {
               var priceBack,sizeBack,priceLay,sizeLay;
               priceBack=tupledArg1[0];
               sizeBack=tupledArg1[1];
               priceLay=tupledArg1[2];
               sizeLay=tupledArg1[3];
               op_EqualsEqualsGreater(runner.backPrice,priceBack);
               op_EqualsEqualsGreater(runner.backSize,sizeBack);
               op_EqualsEqualsGreater(runner.layPrice,priceLay);
               return op_EqualsEqualsGreater(runner.laySize,sizeLay);
              };
              return Option.iter(action2,x4);
             };
             return Seq.iter(action1,x2);
            };
            Seq.iter(action,markets);
            return Concurrency.Return(null);
           });
          }));
         });
        }
       else
        {
         _=Concurrency.Return(null);
        }
       return _;
      });
     }),
     processMarkets:Runtime.Field(function()
     {
      return Concurrency.Delay(function()
      {
       var _,x,chooser,gamesWithoutMarkets,_1,gameId,_arg00_,_arg01_,x1;
       if(State.notCouponMode())
        {
         _=Concurrency.Return(null);
        }
       else
        {
         x=Var1.Get(State.meetups().Var);
         chooser=function(m)
         {
          var binder,option;
          binder=function(_arg1)
          {
           return _arg1.markets.$==0?{
            $:1,
            $0:m.game.gameId
           }:{
            $:0
           };
          };
          option=(State.mtpevt())(m);
          return Option.bind(binder,option);
         };
         gamesWithoutMarkets=Seq.choose(chooser,x);
         if(Seq.isEmpty(gamesWithoutMarkets)?true:ServerBetfairsSession.hasNot())
          {
           _1=Concurrency.Return(null);
          }
         else
          {
           gameId=Seq.head(gamesWithoutMarkets);
           _arg00_=gameId[0];
           _arg01_=gameId[1];
           x1=AjaxRemotingProvider.Async("WebFace:5",[_arg00_,_arg01_]);
           _1=Concurrency.Bind(x1,function(_arg2)
           {
            var action;
            action=function(markets)
            {
             var arg00,arg002;
             arg00=function(evt)
             {
              var markets1;
              markets1=List.map(function(tupledArg)
              {
               var arg001,arg01,arg02,arg03;
               arg001=tupledArg[0];
               arg01=tupledArg[1];
               arg02=tupledArg[2];
               arg03=tupledArg[3];
               return Market.New(arg001,arg01,arg02,arg03);
              },markets);
              return{
               $:1,
               $0:Runtime.New(Event,{
                gameId:evt.gameId,
                country:evt.country,
                markets:markets1
               })
              };
             };
             State.events().UpdateBy(arg00,gameId);
             arg002=function(meetup)
             {
              var marketsBook;
              marketsBook=List.map(function(x2)
              {
               var arg001,arg01,arg02,arg03,arg003;
               arg001=x2[0];
               arg01=x2[1];
               arg02=x2[2];
               arg03=x2[3];
               arg003=Market.New(arg001,arg01,arg02,arg03);
               return MarketBook.New(arg003);
              },markets);
              return{
               $:1,
               $0:Runtime.New(Meetup1,{
                game:meetup.game,
                playMinute:meetup.playMinute,
                status:meetup.status,
                summary:meetup.summary,
                order:meetup.order,
                winBack:meetup.winBack,
                winLay:meetup.winLay,
                drawBack:meetup.drawBack,
                drawLay:meetup.drawLay,
                loseBack:meetup.loseBack,
                loseLay:meetup.loseLay,
                marketsBook:marketsBook,
                totalMatched:meetup.totalMatched,
                country:meetup.country,
                hash:meetup.hash
               })
              };
             };
             return State.meetups().UpdateBy(arg002,gameId);
            };
            Option.iter(action,_arg2);
            return Concurrency.Return(null);
           });
          }
         _=_1;
        }
       return _;
      });
     }),
     processTotalMatched:Runtime.Field(function()
     {
      return Concurrency.Delay(function()
      {
       var _,x,chooser,gamesWithMarkets;
       if(State.notCouponMode())
        {
         _=Concurrency.Return(null);
        }
       else
        {
         x=Var1.Get(State.meetups().Var);
         chooser=function(m)
         {
          var mapping,binder,option,option1;
          mapping=function(evt)
          {
           return evt.markets;
          };
          binder=function(_arg1)
          {
           return _arg1.$==0?{
            $:0
           }:{
            $:1,
            $0:m.game.gameId
           };
          };
          option=(State.mtpevt())(m);
          option1=Option.map(mapping,option);
          return Option.bind(binder,option1);
         };
         gamesWithMarkets=Seq.choose(chooser,x);
         _=(Seq.isEmpty(gamesWithMarkets)?true:ServerBetfairsSession.hasNot())?Concurrency.Return(null):Concurrency.Combine(Concurrency.For(gamesWithMarkets,function(_arg2)
         {
          var _1,_arg00_,_arg01_;
          if(State.notCouponMode())
           {
            _1=Concurrency.Return(null);
           }
          else
           {
            _arg00_=_arg2[0];
            _arg01_=_arg2[1];
            _1=Concurrency.Bind(AjaxRemotingProvider.Async("WebFace:6",[_arg00_,_arg01_]),function(_arg3)
            {
             var _2,_arg00_1,_arg01_1;
             if(_arg3.$==0)
              {
               _2=Concurrency.Return(null);
              }
             else
              {
               _arg00_1=_arg2[0];
               _arg01_1=_arg2[1];
               Work.updateTotalMatched(_arg00_1,_arg01_1,_arg3);
               _2=Concurrency.Return(null);
              }
             return _2;
            });
           }
          return _1;
         }),Concurrency.Delay(function()
         {
          Work.updateColumnGpbVisible();
          return Concurrency.Return(null);
         }));
        }
       return _;
      });
     }),
     start:function(varDataRecived)
     {
      var op_EqualsEqualsGreater;
      op_EqualsEqualsGreater=function(conf,work)
      {
       var arg00,arg01,arg02;
       arg00=conf[0];
       arg01=conf[1];
       arg02=conf[2];
       return Work1["new"](arg00,arg01,arg02,work);
      };
      op_EqualsEqualsGreater(["COUPON",0,0],Work.processCoupon(varDataRecived));
      op_EqualsEqualsGreater(["CHECK-SERVER-BETFAIRS-SESSION",0,0],ServerBetfairsSession.check());
      op_EqualsEqualsGreater(["EVENTS-CATALOGUE",0,0],Work.processEvents());
      op_EqualsEqualsGreater(["MARKETS-CATALOGUE",0,0],Work.processMarkets());
      op_EqualsEqualsGreater(["TOTAL-MATCHED",0,0],Work.processTotalMatched());
      return op_EqualsEqualsGreater(["GAME-DETAIL",0,0],Work.processGameDetail());
     },
     updateColumnCountryVisible:function()
     {
      var x,predicate,value;
      x=Var1.Get(State.meetups().Var);
      predicate=function(x1)
      {
       var binder,option,_arg1,_,x2,_1;
       binder=function(evt)
       {
        return evt.country;
       };
       option=(State.mtpevt())(x1);
       _arg1=Option.bind(binder,option);
       if(_arg1.$==1)
        {
         x2=_arg1.$0;
         if(x2!=="")
          {
           _arg1.$0;
           _1=true;
          }
         else
          {
           _1=false;
          }
         _=_1;
        }
       else
        {
         _=false;
        }
       return _;
      };
      value=Seq.exists(predicate,x);
      return Utils.updateVarValue(State.varColumnCountryVisible(),value);
     },
     updateColumnGpbVisible:function()
     {
      var x,predicate,value;
      x=Var1.Get(State.meetups().Var);
      predicate=function(x1)
      {
       var mapping,predicate1,source,source1;
       mapping=function(tuple)
       {
        return tuple[1];
       };
       predicate1=function(y)
       {
        return 0<y;
       };
       source=Var1.Get(x1.totalMatched);
       source1=Seq.map(mapping,source);
       return Seq.exists(predicate1,source1);
      };
      value=Seq.exists(predicate,x);
      return Utils.updateVarValue(State.varColumnGpbVisible(),value);
     },
     updateCurentCoupon:Runtime.Field(function()
     {
      var chooser;
      chooser=function(tupledArg)
      {
       var gameId,x,y,mapping,option;
       gameId=tupledArg[0];
       x=tupledArg[1];
       y=tupledArg[2];
       mapping=function(m)
       {
        return[m,x,y];
       };
       option=State.meetups().TryFindByKey(gameId);
       return Option.map(mapping,option);
      };
      return function(x)
      {
       var list;
       list=List.choose(chooser,x);
       return Seq.iter(function(tupledArg)
       {
        var x1,i,hash;
        x1=tupledArg[0];
        i=tupledArg[1];
        hash=tupledArg[2];
        return Work.updateGameInfo(x1,i,hash);
       },list);
      };
     }),
     updateGameInfo:function(x,i,hash)
     {
      x.hash=hash;
      Utils.updateVarValue(x.playMinute,i.playMinute);
      Utils.updateVarValue(x.status,i.status);
      Utils.updateVarValue(x.summary,i.summary);
      Utils.updateVarValue(x.order,i.order);
      Utils.updateVarValue(x.winBack,i.winBack);
      Utils.updateVarValue(x.winLay,i.winLay);
      Utils.updateVarValue(x.drawBack,i.drawBack);
      Utils.updateVarValue(x.drawLay,i.drawLay);
      Utils.updateVarValue(x.loseBack,i.loseBack);
      return Utils.updateVarValue(x.loseLay,i.loseLay);
     },
     updateTotalMatched:function(_,_1,_2)
     {
      var gameId,action,option;
      gameId=[_,_1];
      action=function(mtp)
      {
       return Utils.updateVarValue(mtp.totalMatched,_2);
      };
      option=State.meetups().TryFindByKey(gameId);
      return Option.iter(action,option);
     }
    }
   }
  }
 });
 Runtime.OnInit(function()
 {
  UI=Runtime.Safe(Global.WebSharper.UI);
  Next=Runtime.Safe(UI.Next);
  Doc=Runtime.Safe(Next.Doc);
  List=Runtime.Safe(Global.WebSharper.List);
  CentBet=Runtime.Safe(Global.CentBet);
  Client=Runtime.Safe(CentBet.Client);
  Admin=Runtime.Safe(Client.Admin);
  T=Runtime.Safe(List.T);
  AttrModule=Runtime.Safe(Next.AttrModule);
  AttrProxy=Runtime.Safe(Next.AttrProxy);
  Key=Runtime.Safe(Next.Key);
  Var=Runtime.Safe(Next.Var);
  Concurrency=Runtime.Safe(Global.WebSharper.Concurrency);
  Var1=Runtime.Safe(Next.Var1);
  Option=Runtime.Safe(Global.WebSharper.Option);
  Seq=Runtime.Safe(Global.WebSharper.Seq);
  View=Runtime.Safe(Next.View);
  RecordType=Runtime.Safe(Admin.RecordType);
  Strings=Runtime.Safe(Global.WebSharper.Strings);
  Seq1=Runtime.Safe(Global.Seq);
  Remoting=Runtime.Safe(Global.WebSharper.Remoting);
  AjaxRemotingProvider=Runtime.Safe(Remoting.AjaxRemotingProvider);
  Unchecked=Runtime.Safe(Global.WebSharper.Unchecked);
  PrintfHelpers=Runtime.Safe(Global.WebSharper.PrintfHelpers);
  console=Runtime.Safe(Global.console);
  Storage1=Runtime.Safe(Next.Storage1);
  Json=Runtime.Safe(Global.WebSharper.Json);
  Provider=Runtime.Safe(Json.Provider);
  Id=Runtime.Safe(Provider.Id);
  ListModel=Runtime.Safe(Next.ListModel);
  Work=Runtime.Safe(Client.Work);
  State=Runtime.Safe(Client.State);
  GameDetail=Runtime.Safe(Client.GameDetail);
  Coupon=Runtime.Safe(Client.Coupon);
  PageLen=Runtime.Safe(State.PageLen);
  SettingsDialog=Runtime.Safe(Coupon.SettingsDialog);
  Helpers=Runtime.Safe(SettingsDialog.Helpers);
  Utils=Runtime.Safe(Client.Utils);
  Meetup=Runtime.Safe(Client.Meetup);
  View1=Runtime.Safe(Next.View1);
  Operators=Runtime.Safe(Global.WebSharper.Operators);
  Football=Runtime.Safe(Client.Football);
  Market=Runtime.Safe(Football.Market);
  MarketBook=Runtime.Safe(Football.MarketBook);
  RunnerBook=Runtime.Safe(Football.RunnerBook);
  Option1=Runtime.Safe(Utils.Option);
  Helpers1=Runtime.Safe(GameDetail.Helpers);
  LocalStorage=Runtime.Safe(Utils.LocalStorage);
  ServerBetfairsSession=Runtime.Safe(State.ServerBetfairsSession);
  Event=Runtime.Safe(Football.Event);
  Meetup1=Runtime.Safe(Football.Meetup);
  Date=Runtime.Safe(Global.Date);
  JSON=Runtime.Safe(Global.JSON);
  window=Runtime.Safe(Global.window);
  Slice=Runtime.Safe(Global.WebSharper.Slice);
  MatchFailureException=Runtime.Safe(Global.WebSharper.MatchFailureException);
  Work1=Runtime.Safe(Work.Work);
  Collections=Runtime.Safe(Global.WebSharper.Collections);
  return MapModule=Runtime.Safe(Collections.MapModule);
 });
 Runtime.OnLoad(function()
 {
  Work.updateCurentCoupon();
  Work.processTotalMatched();
  Work.processMarkets();
  Work.processGameDetail();
  Work.processEvents();
  Utils.initializeModalVar();
  LocalStorage.strg();
  State.varTargetPageNumber();
  State.varPagesCount();
  State.varMode();
  State.varGameDetailError();
  State.varCurrentPageNumber();
  State.varColumnGpbVisible();
  State.varColumnCountryVisible();
  State.tryGetMarkets();
  State.tryGetCountry();
  State.mtpevt();
  State.meetups();
  State.events();
  ServerBetfairsSession.hasServerBetfairsSession();
  ServerBetfairsSession.check();
  PageLen.view();
  PageLen["var"]();
  PageLen.localStorageKey();
  GameDetail.varError();
  Coupon["render\u0421oupon"]();
  Coupon.renderPagination();
  Coupon.renderMeetup();
  SettingsDialog.elt();
  Helpers.varVisible();
  Helpers.renderDialog();
  Helpers.content();
  Admin.varConsole();
  Admin.varCommandsHistory();
  Admin.varCmd();
  Admin.renderRecord();
  Admin.op_SpliceUntyped();
  Admin["cmd-input"]();
  return;
 });
}());
