(function()
{
 var Global=this,Runtime=this.IntelliFactory.Runtime,UI,Next,Doc,List,CentBet,Client,Admin,T,AttrModule,AttrProxy,Key,Var,Concurrency,Var1,Option,Seq,View,RecordType,Strings,Seq1,Remoting,AjaxRemotingProvider,Unchecked,PrintfHelpers,console,Storage1,Json,Provider,Id,ListModel,Coupon,PageLen,Utils,LocalStorage,Work,ServerBetfairsSession,SettingsDialog,Football,Meetup,EventCatalogue,Collections,MapModule,Meetup1,View1,Operators,Date,JSON,window,FSharpSet,BalancedTree,Slice,MatchFailureException;
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
      var _arg00_69_2;
      _arg00_69_2=function(r)
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
       _arg00_=View.Map(_arg00_69_2,x);
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
         x1=AjaxRemotingProvider.Async("WebFace:1",[inputText1]);
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
     Render:function()
     {
      var varDataRecived,tupledArg,arg00,arg01,arg02,arg10,tupledArg1,arg001,arg011,arg021,arg101,tupledArg2,arg002,arg012,arg022,arg102,tupledArg3,arg003,arg013,arg023,arg103,tupledArg4,arg004,arg014,arg024,arg104,arg005,arg105,_arg00_;
      varDataRecived=Var.Create(false);
      tupledArg=["COUPON",0,0];
      arg00=tupledArg[0];
      arg01=tupledArg[1];
      arg02=tupledArg[2];
      arg10=Coupon.processCoupon(varDataRecived);
      Work["new"](arg00,arg01,arg02,arg10);
      tupledArg1=["CHECK-SERVER-BETFAIRS-SESSION",0,0];
      arg001=tupledArg1[0];
      arg011=tupledArg1[1];
      arg021=tupledArg1[2];
      arg101=ServerBetfairsSession.check();
      Work["new"](arg001,arg011,arg021,arg101);
      tupledArg2=["EVENTS-CATALOGUE",0,0];
      arg002=tupledArg2[0];
      arg012=tupledArg2[1];
      arg022=tupledArg2[2];
      arg102=Coupon.processEvents();
      Work["new"](arg002,arg012,arg022,arg102);
      tupledArg3=["MARKETS-CATALOGUE",0,0];
      arg003=tupledArg3[0];
      arg013=tupledArg3[1];
      arg023=tupledArg3[2];
      arg103=Coupon.processMarkets();
      Work["new"](arg003,arg013,arg023,arg103);
      tupledArg4=["TOTAL-MATCHED",0,0];
      arg004=tupledArg4[0];
      arg014=tupledArg4[1];
      arg024=tupledArg4[2];
      arg104=Coupon.processTotalMatched();
      Work["new"](arg004,arg014,arg024,arg104);
      arg005=function(_arg1)
      {
       var _,arg20;
       if(_arg1)
        {
         _=Coupon["render\u0421oupon"]();
        }
       else
        {
         arg20=List.ofArray([Doc.TextNode("\u0414\u0430\u043d\u043d\u044b\u0435 \u0437\u0430\u0433\u0440\u0443\u0436\u0430\u044e\u0442\u0441\u044f \u0441 \u0441\u0435\u0440\u0432\u0435\u0440\u0430. \u041f\u043e\u0436\u0430\u043b\u0443\u0439\u0441\u0442\u0430, \u043f\u043e\u0434\u043e\u0436\u0434\u0438\u0442\u0435.")]);
         _=Doc.Element("h1",[],arg20);
        }
       return _;
      };
      arg105=varDataRecived.get_View();
      _arg00_=View.Map(arg005,arg105);
      return Doc.EmbedView(_arg00_);
     },
     ServerBetfairsSession:{
      check:Runtime.Field(function()
      {
       return Concurrency.Delay(function()
       {
        return Concurrency.Bind(AjaxRemotingProvider.Async("WebFace:2",[]),function(_arg1)
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
     SettingsDialog:{
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
          x1=SettingsDialog.buttonElt(n,pageLen);
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
       return Doc.Element("button",List.ofArray([SettingsDialog.op_SpliceUntyped("w3-btn w3-teal"),AttrProxy.Create("style","margin: 10px; width: 50px; height: 50px;"),AttrModule.Handler("click",function()
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
      "id'":Runtime.Field(function()
      {
       return"id-settings-dialog";
      }),
      op_SpliceUntyped:function(arg00)
      {
       return AttrProxy.Create("class",arg00);
      },
      render:Runtime.Field(function()
      {
       var ats,ats1,ats2,arg10,arg20,arg00,arg101;
       ats=List.ofArray([SettingsDialog.op_SpliceUntyped("w3-modal"),AttrProxy.Create("id",SettingsDialog["id'"]())]);
       ats1=List.ofArray([SettingsDialog.op_SpliceUntyped("w3-modal-content w3-animate-zoom w3-card-8")]);
       ats2=List.ofArray([SettingsDialog.op_SpliceUntyped("w3-container w3-teal")]);
       arg10="document.getElementById('"+PrintfHelpers.toSafe(SettingsDialog["id'"]())+"').style.display='none'";
       arg20=List.ofArray([Doc.TextNode("\u041a\u043e\u043b\u0438\u0447\u0435\u0441\u0442\u0432\u043e \u043c\u0430\u0442\u0447\u0435\u0439 \u043d\u0430 \u0441\u0442\u0440\u0430\u043d\u0438\u0446\u0435")]);
       arg00=function(value)
       {
        return Global.String(value);
       };
       arg101=PageLen.view();
       return Doc.Element("div",ats,List.ofArray([Doc.Element("div",ats1,List.ofArray([Doc.Element("header",ats2,List.ofArray([Doc.Element("span",List.ofArray([SettingsDialog.op_SpliceUntyped("w3-closebtn"),AttrProxy.Create("onclick",arg10)]),List.ofArray([Doc.TextNode("×")])),Doc.Element("h2",[],arg20)])),Doc.Element("div",List.ofArray([SettingsDialog.op_SpliceUntyped("w3-xxlarge"),AttrProxy.Create("style","margin : 10px; float : left;")]),List.ofArray([Doc.TextView(View.Map(arg00,arg101))])),SettingsDialog.buttonDoc(true),SettingsDialog.buttonDoc(false)]))]));
      })
     },
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
         return Work.loop(x);
        }));
       });
      },
      "new":function(what,sleepInterval,sleepErrorInterval,work)
      {
       var arg00;
       arg00=Runtime.New(Work,{
        what:what,
        sleepInterval:sleepInterval,
        sleepErrorInterval:sleepErrorInterval,
        work:work
       });
       return Work.run(arg00);
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
        return Concurrency.Bind(Work.loop(x),function()
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
      source=Var1.Get(Coupon.meetups().Var);
      existedMeetups=Seq.toList(source);
      Coupon.meetups().Clear();
      mapping=function(tupledArg)
      {
       var game,i,hash,tupledArg1,_arg00_,_arg01_,country,playMinute,status,summary,order,winBack,winLay,drawBack,drawLay,loseBack,loseLay,tupledArg2,_arg00_1,_arg01_1,arg00;
       game=tupledArg[0];
       i=tupledArg[1];
       hash=tupledArg[2];
       tupledArg1=game.gameId;
       _arg00_=tupledArg1[0];
       _arg01_=tupledArg1[1];
       country=Coupon.tryGetCountry(_arg00_,_arg01_);
       playMinute=Var.Create(i.playMinute);
       status=Var.Create(i.status);
       summary=Var.Create(i.summary);
       order=Var.Create(i.order);
       winBack=Var.Create(i.winBack);
       winLay=Var.Create(i.winLay);
       drawBack=Var.Create(i.drawBack);
       drawLay=Var.Create(i.drawLay);
       loseBack=Var.Create(i.loseBack);
       loseLay=Var.Create(i.loseLay);
       tupledArg2=game.gameId;
       _arg00_1=tupledArg2[0];
       _arg01_1=tupledArg2[1];
       arg00=Coupon.getLastTotakMatched(_arg00_1,_arg01_1);
       return Runtime.New(Meetup,{
        game:game,
        playMinute:playMinute,
        status:status,
        summary:summary,
        order:order,
        winBack:winBack,
        winLay:winLay,
        drawBack:drawBack,
        drawLay:drawLay,
        loseBack:loseBack,
        loseLay:loseLay,
        country:Var.Create(country),
        totalMatched:Var.Create(arg00),
        hash:hash
       });
      };
      projection=function(x)
      {
       return Var1.Get(x.order);
      };
      action=function(arg00)
      {
       return Coupon.meetups().Add(arg00);
      };
      source2=List.map(mapping,newGames);
      source1=Seq.append(existedMeetups,source2);
      source3=Seq.sortBy(projection,source1);
      return Seq.iter(action,source3);
     },
     doc:function(x)
     {
      return x;
     },
     eventsCatalogue:Runtime.Field(function()
     {
      var _dt_18_1,_x_19_7,_,arg00,arg10,enc,e,clo1,arg002,arg101,clo11;
      _dt_18_1=LocalStorage.checkTodayKey("CentBetEventsCatalogueCreated","CentBetEventsCatalogue");
      try
      {
       arg00=function(arg001)
       {
        return EventCatalogue.id(arg001);
       };
       enc=(Provider.get_Default().EncodeRecord(EventCatalogue,[["gameId",Provider.get_Default().EncodeTuple([Id,Id]),0],["country",Id,1],["markets",Provider.get_Default().EncodeList(Provider.get_Default().EncodeRecord(undefined,[["marketId",Id,0],["marketName",Id,0],["totalMatched",Id,1],["runners",Provider.get_Default().EncodeList(Provider.get_Default().EncodeRecord(undefined,[["selectionId",Id,0],["runnerName",Id,0]])),0]])),0]]))();
       arg10=Storage1.LocalStorage("CentBetEventsCatalogue",{
        Encode:enc,
        Decode:(Provider.get_Default().DecodeRecord(EventCatalogue,[["gameId",Provider.get_Default().DecodeTuple([Id,Id]),0],["country",Id,1],["markets",Provider.get_Default().DecodeList(Provider.get_Default().DecodeRecord(undefined,[["marketId",Id,0],["marketName",Id,0],["totalMatched",Id,1],["runners",Provider.get_Default().DecodeList(Provider.get_Default().DecodeRecord(undefined,[["selectionId",Id,0],["runnerName",Id,0]])),0]])),0]]))()
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
        return EventCatalogue.id(arg001);
       };
       arg101=Runtime.New(T,{
        $:0
       });
       _=ListModel.Create(arg002,arg101);
      }
      _x_19_7=_;
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
      ((clo11("CentBetEventsCatalogue"))(_x_19_7.get_Length()))(_dt_18_1);
      return _x_19_7;
     }),
     getGamesEvents:function()
     {
      var events,mapping,source,source1;
      events=Var1.Get(Coupon.eventsCatalogue().Var);
      mapping=function(m)
      {
       var predicate;
       predicate=function(e)
       {
        return Unchecked.Equals(e.gameId,m.game.gameId);
       };
       return[m.game.gameId,Seq.tryFind(predicate,events)];
      };
      source=Var1.Get(Coupon.meetups().Var);
      source1=Seq.map(mapping,source);
      return Seq.toList(source1);
     },
     getLastTotakMatched:function(_,_1)
     {
      var gameId,binder,option;
      gameId=[_,_1];
      binder=function(e)
      {
       var chooser,list,matchValue,_2,arg0;
       chooser=function(m)
       {
        return m.totalMatched;
       };
       list=e.markets;
       matchValue=List.choose(chooser,list);
       if(matchValue.$==0)
        {
         _2={
          $:0
         };
        }
       else
        {
         arg0=Seq.sum(matchValue);
         _2={
          $:1,
          $0:arg0
         };
        }
       return _2;
      };
      option=Coupon.eventsCatalogue().TryFindByKey(gameId);
      return Option.bind(binder,option);
     },
     meetups:Runtime.Field(function()
     {
      var _arg00_31_3,_arg10_31_4;
      _arg00_31_3=function(arg00)
      {
       return Meetup.id(arg00);
      };
      _arg10_31_4=Runtime.New(T,{
       $:0
      });
      return ListModel.Create(_arg00_31_3,_arg10_31_4);
     }),
     processCoupon:function(varDataRecived)
     {
      return Concurrency.Delay(function()
      {
       var mapping,source,source1,request,pagelen,x;
       mapping=function(m)
       {
        return[m.game.gameId,m.hash];
       };
       source=Var1.Get(Coupon.meetups().Var);
       source1=Seq.map(mapping,source);
       request=Seq.toList(source1);
       pagelen=PageLen.get();
       x=AjaxRemotingProvider.Async("WebFace:0",[request,Var1.Get(Coupon.varTargetPageNumber()),pagelen]);
       return Concurrency.Bind(x,function(_arg1)
       {
        var updGms,outGms,newGms,gamesCount,pagesCount,a,_;
        updGms=_arg1[1];
        outGms=_arg1[2];
        newGms=_arg1[0];
        gamesCount=_arg1[3];
        pagesCount=(gamesCount/pagelen>>0)+(gamesCount%pagelen===0?0:1);
        if(Var1.Get(Coupon.varPagesCount())!==pagesCount)
         {
          Var1.Set(Coupon.varPagesCount(),pagesCount);
          _=Concurrency.Return(null);
         }
        else
         {
          _=Concurrency.Return(null);
         }
        a=_;
        return Concurrency.Combine(a,Concurrency.Delay(function()
        {
         var a1,_1;
         if(Var1.Get(Coupon.varTargetPageNumber())>pagesCount)
          {
           Var1.Set(Coupon.varTargetPageNumber(),0);
           _1=Concurrency.Return(null);
          }
         else
          {
           _1=Concurrency.Return(null);
          }
         a1=_1;
         return Concurrency.Combine(a1,Concurrency.Delay(function()
         {
          var a2,value,_2;
          value=Var1.Get(varDataRecived);
          if(!value)
           {
            Var1.Set(varDataRecived,true);
            _2=Concurrency.Return(null);
           }
          else
           {
            _2=Concurrency.Return(null);
           }
          a2=_2;
          return Concurrency.Combine(a2,Concurrency.Delay(function()
          {
           var _3;
           if(Var1.Get(Coupon.varCurrentPageNumber())!==Var1.Get(Coupon.varTargetPageNumber()))
            {
             Var1.Set(Coupon.varCurrentPageNumber(),Var1.Get(Coupon.varTargetPageNumber()));
             _3=Concurrency.Return(null);
            }
           else
            {
             _3=Concurrency.Return(null);
            }
           return Concurrency.Combine(_3,Concurrency.Delay(function()
           {
            Coupon.updateCoupon(newGms,updGms,outGms);
            return Concurrency.Return(null);
           }));
          }));
         }));
        }));
       });
      });
     },
     processEvents:Runtime.Field(function()
     {
      return Concurrency.Delay(function()
      {
       var chooser,list,gamesWithoutEvent,_1,x;
       Coupon.updateColumnCountryVisible();
       chooser=function(_arg1)
       {
        var _,gameId;
        if(_arg1[1].$==0)
         {
          gameId=_arg1[0];
          _={
           $:1,
           $0:gameId
          };
         }
        else
         {
          _={
           $:0
          };
         }
        return _;
       };
       list=Coupon.getGamesEvents();
       gamesWithoutEvent=List.choose(chooser,list);
       if(gamesWithoutEvent.$==0?true:ServerBetfairsSession.hasNot())
        {
         _1=Concurrency.Return(null);
        }
       else
        {
         x=AjaxRemotingProvider.Async("WebFace:3",[gamesWithoutEvent]);
         _1=Concurrency.Bind(x,function(_arg2)
         {
          var a;
          a=Concurrency.For(_arg2,function(_arg3)
          {
           var gameId,country;
           _arg3[1];
           gameId=_arg3[0];
           country=_arg3[2];
           Coupon.eventsCatalogue().Add(Runtime.New(EventCatalogue,{
            gameId:gameId,
            country:country,
            markets:Runtime.New(T,{
             $:0
            })
           }));
           return Concurrency.Return(null);
          });
          return Concurrency.Combine(a,Concurrency.Delay(function()
          {
           return Concurrency.For(Var1.Get(Coupon.meetups().Var),function(_arg4)
           {
            var tupledArg,_arg00_,_arg01_;
            tupledArg=_arg4.game.gameId;
            _arg00_=tupledArg[0];
            _arg01_=tupledArg[1];
            Var1.Set(_arg4.country,Coupon.tryGetCountry(_arg00_,_arg01_));
            return Concurrency.Return(null);
           });
          }));
         });
        }
       return _1;
      });
     }),
     processMarkets:Runtime.Field(function()
     {
      return Concurrency.Delay(function()
      {
       var chooser,list,gamesWithoutMarkets;
       chooser=function(_arg1)
       {
        var _,_1,gameId;
        if(_arg1[1].$==1)
         {
          if(_arg1[1].$0.markets.$==0)
           {
            gameId=_arg1[0];
            _1={
             $:1,
             $0:gameId
            };
           }
          else
           {
            _1={
             $:0
            };
           }
          _=_1;
         }
        else
         {
          _={
           $:0
          };
         }
        return _;
       };
       list=Coupon.getGamesEvents();
       gamesWithoutMarkets=List.choose(chooser,list);
       return(gamesWithoutMarkets.$==0?true:ServerBetfairsSession.hasNot())?Concurrency.Return(null):Concurrency.For(gamesWithoutMarkets,function(_arg2)
       {
        var _arg00_,_arg01_;
        _arg00_=_arg2[0];
        _arg01_=_arg2[1];
        return Concurrency.Bind(AjaxRemotingProvider.Async("WebFace:4",[_arg00_,_arg01_]),function(_arg3)
        {
         var _,values,_arg00_1,_arg01_1;
         if(_arg3.$==1)
          {
           values=_arg3.$0;
           _arg00_1=_arg2[0];
           _arg01_1=_arg2[1];
           Coupon.setMarket(_arg00_1,_arg01_1,values);
           _=Concurrency.Return(null);
          }
         else
          {
           _=Concurrency.Return(null);
          }
         return _;
        });
       });
      });
     }),
     processTotalMatched:Runtime.Field(function()
     {
      return Concurrency.Delay(function()
      {
       var chooser,list,gamesWithMarkets;
       Coupon.updateColumnGpbVisible();
       chooser=function(_arg1)
       {
        var _,_1,gameId;
        if(_arg1[1].$==1)
         {
          if(_arg1[1].$0.markets.$==1)
           {
            gameId=_arg1[0];
            _1={
             $:1,
             $0:gameId
            };
           }
          else
           {
            _1={
             $:0
            };
           }
          _=_1;
         }
        else
         {
          _={
           $:0
          };
         }
        return _;
       };
       list=Coupon.getGamesEvents();
       gamesWithMarkets=List.choose(chooser,list);
       return(gamesWithMarkets.$==0?true:ServerBetfairsSession.hasNot())?Concurrency.Return(null):Concurrency.For(gamesWithMarkets,function(_arg2)
       {
        var _arg00_,_arg01_,x;
        _arg00_=_arg2[0];
        _arg01_=_arg2[1];
        x=AjaxRemotingProvider.Async("WebFace:5",[_arg00_,_arg01_]);
        return Concurrency.Bind(x,function(_arg3)
        {
         var _,folder,_arg00_1,_arg01_1,toltalMatched;
         if(_arg3.get_IsEmpty())
          {
           _=Concurrency.Return(null);
          }
         else
          {
           folder=function(acc)
           {
            return function()
            {
             return function(value)
             {
              return acc+value;
             };
            };
           };
           _arg00_1=_arg2[0];
           _arg01_1=_arg2[1];
           toltalMatched=MapModule.Fold(folder,0,_arg3);
           Coupon.updateTotalMatched(_arg00_1,_arg01_1,toltalMatched);
           _=Concurrency.Return(null);
          }
         return _;
        });
       });
      });
     }),
     renderMeetup:Runtime.Field(function()
     {
      var tupledArg,viewColumnGpbVisible,viewColumnCountryVisible;
      tupledArg=[Coupon.varColumnGpbVisible().get_View(),Coupon.varColumnCountryVisible().get_View()];
      viewColumnGpbVisible=tupledArg[0];
      viewColumnCountryVisible=tupledArg[1];
      return function(x)
      {
       return Meetup1.renderMeetup(viewColumnGpbVisible,viewColumnCountryVisible,x);
      };
     }),
     renderPagination:Runtime.Field(function()
     {
      var _builder_158_3,x,_arg00_;
      _builder_158_3=View.get_Do();
      x=Coupon.varPagesCount().get_View();
      _arg00_=View1.Bind(function(_arg1)
      {
       var x1;
       x1=Coupon.varCurrentPageNumber().get_View();
       return View1.Bind(function(_arg2)
       {
        var x2,aShowDialog,list,arg00;
        x2="document.getElementById('"+PrintfHelpers.toSafe(SettingsDialog["id'"]())+"').style.display='block'";
        aShowDialog=AttrProxy.Create("onclick",x2);
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
              return Var1.Set(Coupon.varTargetPageNumber(),n);
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
        arg00=List.map(function(x3)
        {
         return Coupon.doc(x3);
        },list);
        return View1.Const(Doc.Concat(arg00));
       },x1);
      },x);
      return Doc.EmbedView(_arg00_);
     }),
     "render\u0421oupon":Runtime.Field(function()
     {
      var ats,arg20,ats1,arg201,arg202,x,f,mapping,arg00,x3,_etable_183_4,ats2,_builder_195_2,x4,_arg00_;
      ats=List.ofArray([AttrProxy.Create("class","w3-responsive")]);
      ats1=List.ofArray([AttrProxy.Create("class","w3-table w3-bordered w3-striped w3-hoverable")]);
      arg201=List.ofArray([Doc.Element("tr",List.ofArray([AttrModule.Class("coupon-header-row w3-teal")]),Meetup1.renderGamesHeaderRow(Coupon.varColumnGpbVisible().get_View()))]);
      x=Coupon.meetups().get_View();
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
      _etable_183_4=Doc.Element("div",ats,arg20);
      ats2=List.ofArray([AttrProxy.Create("class","w3-container")]);
      _builder_195_2=View.get_Do();
      x4=Coupon.varCurrentPageNumber().get_View();
      _arg00_=View1.Bind(function(_arg1)
      {
       return View1.Bind(function(_arg2)
       {
        var x1,_,arg203,t;
        if(_arg1===_arg2)
         {
          _=_etable_183_4;
         }
        else
         {
          t="\u0412\u044b\u043f\u043e\u043b\u043d\u044f\u0435\u0442\u0441\u044f \u043f\u0435\u0440\u0435\u0445\u043e\u0434 \u043a \u0441\u0442\u0440\u0430\u043d\u0438\u0446\u0435 \u2116 "+Global.String(_arg2+1)+"...";
          arg203=List.ofArray([Doc.TextNode(t)]);
          _=Doc.Element("h1",[],arg203);
         }
        x1=_;
        return View1.Const(Coupon.doc(x1));
       },Coupon.varTargetPageNumber().get_View());
      },x4);
      return Doc.Element("div",ats2,List.ofArray([Doc.Element("div",List.ofArray([AttrProxy.Create("class","w3-center")]),List.ofArray([Doc.Element("ul",List.ofArray([AttrProxy.Create("class","w3-pagination w3-border w3-round")]),List.ofArray([Coupon.renderPagination()]))])),Doc.EmbedView(_arg00_),SettingsDialog.render()]));
     }),
     setMarket:function(_,_1,_2)
     {
      var gameId,chooser,folder,_arg00_,_arg01_,list,toltalMatched,mapping,markets,arg00;
      gameId=[_,_1];
      chooser=function(tupledArg)
      {
       var x;
       tupledArg[0];
       tupledArg[1];
       tupledArg[2];
       x=tupledArg[3];
       return x;
      };
      folder=function(x)
      {
       return function(y)
       {
        return x+y;
       };
      };
      _arg00_=gameId[0];
      _arg01_=gameId[1];
      list=List.choose(chooser,_2);
      toltalMatched=Seq.fold(folder,0,list);
      Coupon.updateTotalMatched(_arg00_,_arg01_,toltalMatched);
      mapping=function(tupledArg)
      {
       var marketId,marketName,runners,totalMatched,mapping1;
       marketId=tupledArg[0];
       marketName=tupledArg[1];
       runners=tupledArg[2];
       totalMatched=tupledArg[3];
       mapping1=function(tupledArg1)
       {
        var runnerNamem,selectionId;
        runnerNamem=tupledArg1[0];
        selectionId=tupledArg1[1];
        return{
         selectionId:selectionId,
         runnerName:runnerNamem
        };
       };
       return{
        marketId:marketId,
        marketName:marketName,
        totalMatched:totalMatched,
        runners:List.map(mapping1,runners)
       };
      };
      markets=List.map(mapping,_2);
      arg00=function(x)
      {
       return{
        $:1,
        $0:Runtime.New(EventCatalogue,{
         gameId:x.gameId,
         country:x.country,
         markets:markets
        })
       };
      };
      return Coupon.eventsCatalogue().UpdateBy(arg00,gameId);
     },
     stvr:function(x,value)
     {
      return!Unchecked.Equals(Var1.Get(x),value)?Var1.Set(x,value):null;
     },
     tryGetCountry:function(_,_1)
     {
      var gameId,_arg00_,_arg01_,_arg1,_2,_3,country;
      gameId=[_,_1];
      _arg00_=gameId[0];
      _arg01_=gameId[1];
      _arg1=Coupon.tryGetEvent(_arg00_,_arg01_);
      if(_arg1.$==1)
       {
        if(_arg1.$0.country.$==1)
         {
          country=_arg1.$0.country.$0;
          _3=country;
         }
        else
         {
          _3="";
         }
        _2=_3;
       }
      else
       {
        _2="";
       }
      return _2;
     },
     tryGetEvent:function(_,_1)
     {
      var gameId,predicate,source;
      gameId=[_,_1];
      predicate=function(_arg1)
      {
       var _gameId_;
       _gameId_=_arg1.gameId;
       return Unchecked.Equals(gameId,_gameId_);
      };
      source=Var1.Get(Coupon.eventsCatalogue().Var);
      return Seq.tryFind(predicate,source);
     },
     updateColumnCountryVisible:function()
     {
      var predicate,source,hasCountry;
      predicate=function(x)
      {
       return Var1.Get(x.country)!=="";
      };
      source=Var1.Get(Coupon.meetups().Var);
      hasCountry=Seq.exists(predicate,source);
      return!Unchecked.Equals(Var1.Get(Coupon.varColumnCountryVisible()),hasCountry)?Var1.Set(Coupon.varColumnCountryVisible(),hasCountry):null;
     },
     updateColumnGpbVisible:function()
     {
      var predicate,source,hasGpb;
      predicate=function(x)
      {
       return Var1.Get(x.totalMatched).$==1;
      };
      source=Var1.Get(Coupon.meetups().Var);
      hasGpb=Seq.exists(predicate,source);
      return!Unchecked.Equals(Var1.Get(Coupon.varColumnGpbVisible()),hasGpb)?Var1.Set(Coupon.varColumnGpbVisible(),hasGpb):null;
     },
     updateCoupon:function(newGms,updGms,outGms)
     {
      var value,value1,_,action,action1;
      value=newGms.$==0;
      !value?Coupon.addNewGames(newGms):null;
      value1=Seq.isEmpty(outGms);
      if(!value1)
       {
        action=function(arg00)
        {
         return Coupon.meetups().RemoveByKey(arg00);
        };
        _=Seq.iter(action,outGms);
       }
      else
       {
        _=null;
       }
      action1=function(tupledArg)
      {
       var gameId,i,hash,matchValue,_1,x,_2,x1;
       gameId=tupledArg[0];
       i=tupledArg[1];
       hash=tupledArg[2];
       matchValue=Coupon.meetups().TryFindByKey(gameId);
       if(matchValue.$==1)
        {
         x=matchValue.$0;
         if(x.hash!==hash)
          {
           x1=matchValue.$0;
           x1.hash=hash;
           Coupon.stvr(x1.playMinute,i.playMinute);
           Coupon.stvr(x1.status,i.status);
           Coupon.stvr(x1.summary,i.summary);
           Coupon.stvr(x1.order,i.order);
           Coupon.stvr(x1.winBack,i.winBack);
           Coupon.stvr(x1.winLay,i.winLay);
           Coupon.stvr(x1.drawBack,i.drawBack);
           Coupon.stvr(x1.drawLay,i.drawLay);
           Coupon.stvr(x1.loseBack,i.loseBack);
           _2=Coupon.stvr(x1.loseLay,i.loseLay);
          }
         else
          {
           _2=null;
          }
         _1=_2;
        }
       else
        {
         _1=null;
        }
       return _1;
      };
      Seq.iter(action1,updGms);
      Coupon.updateColumnCountryVisible();
      return Coupon.updateColumnGpbVisible();
     },
     updateTotalMatched:function(_,_1,_2)
     {
      var gameId,matchValue,_3,game;
      gameId=[_,_1];
      matchValue=Coupon.meetups().TryFindByKey(gameId);
      if(matchValue.$==1)
       {
        game=matchValue.$0;
        _3=Var1.Set(game.totalMatched,{
         $:1,
         $0:_2
        });
       }
      else
       {
        _3=null;
       }
      return _3;
     },
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
     varPagesCount:Runtime.Field(function()
     {
      return Var.Create(1);
     }),
     varTargetPageNumber:Runtime.Field(function()
     {
      return Var.Create(0);
     })
    },
    Football:{
     EventCatalogue:Runtime.Class({},{
      id:function(x)
      {
       return x.gameId;
      }
     }),
     Meetup:Runtime.Class({},{
      id:function(x)
      {
       return x.game.gameId;
      }
     })
    },
    Meetup:{
     doc:function(x)
     {
      return x;
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
        return Meetup1.doc(x);
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
           _=Meetup1.doc(x);
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
         return[Meetup1.doc(x)];
        }));
       }));
      }));
     },
     renderMeetup:function(viewColumnGpbVisible,viewColumnCountryVisible,x)
     {
      var _kef_,_bck_,_lay_,ch,arg20,arg002,arg101,v2,x2,x3,_builder_,x4,_arg00_,xa,xb,xc,arg003,_arg00_1,xd,arg004,_arg00_2;
      _kef_=function(back)
      {
       return function(sel)
       {
        return function(v)
        {
         var x1,_,arg00,arg001,arg10,v1;
         _=back?"back":"lay";
         arg00="kef "+PrintfHelpers.toSafe(sel)+" "+PrintfHelpers.toSafe(_);
         arg001=function(_arg1)
         {
          return Utils.formatDecimalOption(_arg1);
         };
         arg10=v.get_View();
         v1=View.Map(arg001,arg10);
         x1=Doc.Element("td",List.ofArray([AttrProxy.Create("class",arg00)]),List.ofArray([Doc.TextView(v1)]));
         return Meetup1.doc(x1);
        };
       };
      };
      _bck_=_kef_(true);
      _lay_=_kef_(false);
      arg002=function(tupledArg)
      {
       var page,n;
       page=tupledArg[0];
       n=tupledArg[1];
       return Global.String(page)+"."+Global.String(n);
      };
      arg101=x.order.get_View();
      v2=View.Map(arg002,arg101);
      arg20=List.ofArray([Doc.TextView(v2)]);
      x2=Doc.Element("td",[],arg20);
      x3=Doc.Element("td",List.ofArray([AttrProxy.Create("class","home-team")]),List.ofArray([Doc.TextNode(x.game.home)]));
      _builder_=View.get_Do();
      x4=x.playMinute.get_View();
      _arg00_=View1.Bind(function(_arg1)
      {
       var x1;
       x1=x.summary.get_View();
       return View1.Bind(function(_arg2)
       {
        var _,_1,x5,_2,x6,arg201,x7,_3,x8,arg202,x9;
        if(_arg1.$==1)
         {
          if(_arg2!=="")
           {
            x5=Doc.Element("td",List.ofArray([AttrProxy.Create("class","game-status")]),List.ofArray([Doc.TextNode(_arg2)]));
            _1=Meetup1.doc(x5);
           }
          else
           {
            if(_arg2!=="")
             {
              x6=Doc.Element("td",List.ofArray([AttrProxy.Create("class","game-status")]),List.ofArray([Doc.TextNode(_arg2)]));
              _2=Meetup1.doc(x6);
             }
            else
             {
              arg201=Runtime.New(T,{
               $:0
              });
              x7=Doc.Element("td",[],arg201);
              _2=Meetup1.doc(x7);
             }
            _1=_2;
           }
          _=_1;
         }
        else
         {
          if(_arg2!=="")
           {
            x8=Doc.Element("td",List.ofArray([AttrProxy.Create("class","game-status")]),List.ofArray([Doc.TextNode(_arg2)]));
            _3=Meetup1.doc(x8);
           }
          else
           {
            arg202=Runtime.New(T,{
             $:0
            });
            x9=Doc.Element("td",[],arg202);
            _3=Meetup1.doc(x9);
           }
          _=_3;
         }
        return View1.Const(_);
       },x1);
      },x4);
      xa=Doc.Element("td",List.ofArray([AttrProxy.Create("class","away-team")]),List.ofArray([Doc.TextNode(x.game.away)]));
      xb=Doc.Element("td",List.ofArray([AttrProxy.Create("class","game-status")]),List.ofArray([Doc.TextView(x.status.get_View())]));
      View.get_Do();
      xc=View1.Bind(function(_arg3)
      {
       return View1.Bind(function(_arg4)
       {
        return View1.Const([_arg3,_arg4]);
       },viewColumnGpbVisible);
      },x.totalMatched.get_View());
      arg003=function(_arg5)
      {
       var _,_1,totalMatched,x1,t,arg201,x5;
       if(_arg5[1])
        {
         if(_arg5[0].$==1)
          {
           totalMatched=_arg5[0].$0;
           t=Global.String(totalMatched);
           x1=Doc.Element("td",List.ofArray([AttrProxy.Create("class","game-gpb")]),List.ofArray([Doc.TextNode(t)]));
           _1=Meetup1.doc(x1);
          }
         else
          {
           arg201=Runtime.New(T,{
            $:0
           });
           x5=Doc.Element("td",[],arg201);
           _1=Meetup1.doc(x5);
          }
         _=_1;
        }
       else
        {
         _=Doc.get_Empty();
        }
       return _;
      };
      _arg00_1=View.Map(arg003,xc);
      View.get_Do();
      xd=View1.Bind(function(_arg6)
      {
       return View1.Bind(function(_arg7)
       {
        return View1.Const([_arg6,_arg7]);
       },viewColumnCountryVisible);
      },x.country.get_View());
      arg004=function(_arg8)
      {
       var _,x1;
       if(_arg8[1])
        {
         _arg8[0];
         x1=Doc.Element("td",List.ofArray([AttrProxy.Create("class","game-country")]),List.ofArray([Doc.TextView(x.country.get_View())]));
         _=Meetup1.doc(x1);
        }
       else
        {
         _=Doc.get_Empty();
        }
       return _;
      };
      _arg00_2=View.Map(arg004,xd);
      ch=List.ofArray([Meetup1.doc(x2),Meetup1.doc(x3),Doc.EmbedView(_arg00_),Meetup1.doc(xa),Meetup1.doc(xb),(_bck_("win"))(x.winBack),(_lay_("win"))(x.winLay),(_bck_("draw"))(x.drawBack),(_lay_("draw"))(x.drawLay),(_bck_("lose"))(x.loseBack),(_lay_("lose"))(x.loseLay),Doc.EmbedView(_arg00_1),Doc.EmbedView(_arg00_2)]);
      return Doc.Element("tr",[],ch);
     }
    },
    Utils:{
     Left:function(arg0)
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
     Right:function(arg0)
     {
      return{
       $:1,
       $0:arg0
      };
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
     mkids:function(x,getid)
     {
      var mapping,elements,m,elements1,s;
      mapping=function(g)
      {
       return[getid(g),g];
      };
      elements=List.map(mapping,x);
      m=MapModule.OfArray(Seq.toArray(elements));
      elements1=List.map(getid,x);
      s=FSharpSet.New1(BalancedTree.OfSeq(elements1));
      return[s,m];
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
              _5=Operators.Raise(MatchFailureException.New("E:\\User\\Docs\\Visual Studio 2015\\Projects\\CentBet\\CentBet\\WebFace\\ClientUtils.fs",8,18));
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
     "|Left|Right|":function(_arg1)
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
  Coupon=Runtime.Safe(Client.Coupon);
  PageLen=Runtime.Safe(Coupon.PageLen);
  Utils=Runtime.Safe(Client.Utils);
  LocalStorage=Runtime.Safe(Utils.LocalStorage);
  Work=Runtime.Safe(Coupon.Work);
  ServerBetfairsSession=Runtime.Safe(Coupon.ServerBetfairsSession);
  SettingsDialog=Runtime.Safe(Coupon.SettingsDialog);
  Football=Runtime.Safe(Client.Football);
  Meetup=Runtime.Safe(Football.Meetup);
  EventCatalogue=Runtime.Safe(Football.EventCatalogue);
  Collections=Runtime.Safe(Global.WebSharper.Collections);
  MapModule=Runtime.Safe(Collections.MapModule);
  Meetup1=Runtime.Safe(Client.Meetup);
  View1=Runtime.Safe(Next.View1);
  Operators=Runtime.Safe(Global.WebSharper.Operators);
  Date=Runtime.Safe(Global.Date);
  JSON=Runtime.Safe(Global.JSON);
  window=Runtime.Safe(Global.window);
  FSharpSet=Runtime.Safe(Collections.FSharpSet);
  BalancedTree=Runtime.Safe(Collections.BalancedTree);
  Slice=Runtime.Safe(Global.WebSharper.Slice);
  return MatchFailureException=Runtime.Safe(Global.WebSharper.MatchFailureException);
 });
 Runtime.OnLoad(function()
 {
  LocalStorage.strg();
  Coupon.varTargetPageNumber();
  Coupon.varPagesCount();
  Coupon.varCurrentPageNumber();
  Coupon.varColumnGpbVisible();
  Coupon.varColumnCountryVisible();
  Coupon["render\u0421oupon"]();
  Coupon.renderPagination();
  Coupon.renderMeetup();
  Coupon.processTotalMatched();
  Coupon.processMarkets();
  Coupon.processEvents();
  Coupon.meetups();
  Coupon.eventsCatalogue();
  SettingsDialog.render();
  SettingsDialog["id'"]();
  ServerBetfairsSession.hasServerBetfairsSession();
  ServerBetfairsSession.check();
  PageLen.view();
  PageLen["var"]();
  PageLen.localStorageKey();
  Admin.varConsole();
  Admin.varCommandsHistory();
  Admin.varCmd();
  Admin.renderRecord();
  Admin.op_SpliceUntyped();
  Admin["cmd-input"]();
  return;
 });
}());
