(function()
{
 var Global=this,Runtime=this.IntelliFactory.Runtime,List,CentBet,Client,Admin,T,UI,Next,Doc,AttrModule,AttrProxy,Key,Var,Concurrency,Var1,Option,Seq,View,RecordType,Strings,Seq1,Remoting,AjaxRemotingProvider,Unchecked,Storage1,Json,Provider,Id,ListModel,Coupon,ServerBetfairsSession,Work,PrintfHelpers,console,Meetup,Utils,LocalStorage,EventCatalogue,View1,Operators,Date,JSON,window,Collections,MapModule,FSharpSet,BalancedTree,Slice,MatchFailureException;
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
      var arg00,arg20;
      arg20=Runtime.New(T,{
       $:0
      });
      arg00=List.ofArray([Admin.RenderCommandPrompt(),(Admin.op_SpliceUntyped())(Doc.Element("br",[],arg20)),Admin.RenderRecords()]);
      return Doc.Concat(arg00);
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
       var patternInput,fore,back;
       patternInput=(RecordType.get_color())(r.RecordType);
       fore=patternInput[1];
       back=patternInput[0];
       return(Admin.op_SpliceUntyped())(Doc.Element("span",List.ofArray([AttrModule.Style("color",fore),AttrModule.Style("background",back)]),List.ofArray([Doc.TextNode(r.Text)])));
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
      var _,arg00,arg10,e,arg001,arg101;
      try
      {
       arg00=function(x)
       {
        return Admin.cmdKey(x);
       };
       arg10=Storage1.LocalStorage("CentBetConsoleCommandsHistory",{
        Encode:(Provider.get_Default().EncodeRecord(undefined,[["Id",Provider.get_Default().EncodeUnion(Key,"$",[[0,[["$0","Item",Id,0]]]]),0],["Text",Id,0]]))(),
        Decode:(Provider.get_Default().DecodeRecord(undefined,[["Id",Provider.get_Default().DecodeUnion(Key,"$",[[0,[["$0","Item",Id,0]]]]),0],["Text",Id,0]]))()
       });
       _=ListModel.CreateWithStorage(arg00,arg10);
      }
      catch(e)
      {
       arg001=function(x)
       {
        return Admin.cmdKey(x);
       };
       arg101=Runtime.New(T,{
        $:0
       });
       _=ListModel.Create(arg001,arg101);
      }
      return _;
     }),
     varConsole:Runtime.Field(function()
     {
      var _,arg00,arg10,e,arg001,arg101;
      try
      {
       arg00=function(x)
       {
        return Admin.recordKey(x);
       };
       arg10=Storage1.LocalStorage("CentBetConsole",{
        Encode:(Provider.get_Default().EncodeRecord(undefined,[["Id",Provider.get_Default().EncodeUnion(Key,"$",[[0,[["$0","Item",Id,0]]]]),0],["Text",Id,0],["RecordType",Provider.get_Default().EncodeUnion(RecordType,"$",[[0,[]],[1,[]],[2,[]]]),0]]))(),
        Decode:(Provider.get_Default().DecodeRecord(undefined,[["Id",Provider.get_Default().DecodeUnion(Key,"$",[[0,[["$0","Item",Id,0]]]]),0],["Text",Id,0],["RecordType",Provider.get_Default().DecodeUnion(RecordType,"$",[[0,[]],[1,[]],[2,[]]]),0]]))()
       });
       _=ListModel.CreateWithStorage(arg00,arg10);
      }
      catch(e)
      {
       arg001=function(x)
       {
        return Admin.recordKey(x);
       };
       arg101=Runtime.New(T,{
        $:0
       });
       _=ListModel.Create(arg001,arg101);
      }
      return _;
     })
    },
    Coupon:{
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
     }),
     Render:function()
     {
      var tupledArg,arg00,arg01,arg02,arg10,tupledArg1,arg001,arg011,arg021,arg101,tupledArg2,arg002,arg012,arg022,arg102,tupledArg3,arg003,arg013,arg023,arg103,arg004,arg104,_arg00_1;
      tupledArg=["COUPON",0,0];
      arg00=tupledArg[0];
      arg01=tupledArg[1];
      arg02=tupledArg[2];
      arg10=function()
      {
       return ServerBetfairsSession.check();
      };
      Work["new"](arg00,arg01,arg02,arg10);
      tupledArg1=["CHECK-SERVER-BETFAIRS-SESSION",0,0];
      arg001=tupledArg1[0];
      arg011=tupledArg1[1];
      arg021=tupledArg1[2];
      arg101=function()
      {
       return Coupon.processCoupon();
      };
      Work["new"](arg001,arg011,arg021,arg101);
      tupledArg2=["EVENTS-CATALOGUE",0,0];
      arg002=tupledArg2[0];
      arg012=tupledArg2[1];
      arg022=tupledArg2[2];
      arg102=function()
      {
       return Coupon.processEvents();
      };
      Work["new"](arg002,arg012,arg022,arg102);
      tupledArg3=["MARKETS-CATALOGUE",0,0];
      arg003=tupledArg3[0];
      arg013=tupledArg3[1];
      arg023=tupledArg3[2];
      arg103=function()
      {
       return Coupon.processMarkets();
      };
      Work["new"](arg003,arg013,arg023,arg103);
      arg004=function(_arg1)
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
      arg104=Coupon.varDataRecived().get_View();
      _arg00_1=View.Map(arg004,arg104);
      return Doc.EmbedView(_arg00_1);
     },
     ServerBetfairsSession:{
      check:function()
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
      },
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
     Work:Runtime.Class({},{
      loop:function(x)
      {
       return Concurrency.Delay(function()
       {
        var a;
        a=Concurrency.Delay(function()
        {
         return Concurrency.Bind(x.work.call(null,null),function()
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
       var game,i,hash,tupledArg1,_arg00_,_arg01_,country,playMinute,status,summary,order,winBack,winLay,drawBack,drawLay,loseBack,loseLay;
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
        totalMatched:Var.Create({
         $:0
        }),
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
      var dt,x,_,arg00,arg10,enc,e,clo1,arg002,arg101,clo11;
      dt=LocalStorage.checkTodayKey("CentBetEventsCatalogueCreated","CentBetEventsCatalogue");
      try
      {
       arg00=function(arg001)
       {
        return EventCatalogue.id(arg001);
       };
       enc=(Provider.get_Default().EncodeRecord(EventCatalogue,[["gameId",Provider.get_Default().EncodeTuple([Id,Id]),0],["country",Id,1],["markets",Provider.get_Default().EncodeList(Provider.get_Default().EncodeRecord(undefined,[["marketId",Id,0],["marketName",Id,0],["runners",Provider.get_Default().EncodeList(Provider.get_Default().EncodeRecord(undefined,[["selectionId",Id,0],["runnerName",Id,0]])),0]])),0]]))();
       arg10=Storage1.LocalStorage("CentBetEventsCatalogue",{
        Encode:enc,
        Decode:(Provider.get_Default().DecodeRecord(EventCatalogue,[["gameId",Provider.get_Default().DecodeTuple([Id,Id]),0],["country",Id,1],["markets",Provider.get_Default().DecodeList(Provider.get_Default().DecodeRecord(undefined,[["marketId",Id,0],["marketName",Id,0],["runners",Provider.get_Default().DecodeList(Provider.get_Default().DecodeRecord(undefined,[["selectionId",Id,0],["runnerName",Id,0]])),0]])),0]]))()
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
     meetups:Runtime.Field(function()
     {
      var arg00,arg10;
      arg00=function(arg001)
      {
       return Meetup.id(arg001);
      };
      arg10=Runtime.New(T,{
       $:0
      });
      return ListModel.Create(arg00,arg10);
     }),
     processCoupon:function()
     {
      return Concurrency.Delay(function()
      {
       var mapping,source,source1,request,x;
       mapping=function(m)
       {
        return[m.game.gameId,m.hash];
       };
       source=Var1.Get(Coupon.meetups().Var);
       source1=Seq.map(mapping,source);
       request=Seq.toList(source1);
       x=AjaxRemotingProvider.Async("WebFace:0",[request,Var1.Get(Coupon.varCurrentPageNumber()),30]);
       return Concurrency.Bind(x,function(_arg1)
       {
        var updGms,outGms,newGms,gamesCount,pagesCount,a,_;
        updGms=_arg1[1];
        outGms=_arg1[2];
        newGms=_arg1[0];
        gamesCount=_arg1[3];
        pagesCount=gamesCount/30>>0;
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
         if(Var1.Get(Coupon.varCurrentPageNumber())>pagesCount)
          {
           Var1.Set(Coupon.varCurrentPageNumber(),0);
           _1=Concurrency.Return(null);
          }
         else
          {
           _1=Concurrency.Return(null);
          }
         a1=_1;
         return Concurrency.Combine(a1,Concurrency.Delay(function()
         {
          var _2;
          if(!Var1.Get(Coupon.varDataRecived()))
           {
            Var1.Set(Coupon.varDataRecived(),true);
            _2=Concurrency.Return(null);
           }
          else
           {
            _2=Concurrency.Return(null);
           }
          return Concurrency.Combine(_2,Concurrency.Delay(function()
          {
           Coupon.updateCoupon(newGms,updGms,outGms);
           return Concurrency.Return(null);
          }));
         }));
        }));
       });
      });
     },
     processEvents:function()
     {
      return Concurrency.Delay(function()
      {
       var _events_,x,chooser,source,request,_,x1;
       _events_=Var1.Get(Coupon.eventsCatalogue().Var);
       x=Var1.Get(Coupon.meetups().Var);
       chooser=function(m)
       {
        var predicate,matchValue;
        predicate=function(e)
        {
         return Unchecked.Equals(e.gameId,m.game.gameId);
        };
        matchValue=Seq.tryFind(predicate,_events_);
        return matchValue.$==0?{
         $:1,
         $0:m.game.gameId
        }:{
         $:0
        };
       };
       source=Seq.choose(chooser,x);
       request=Seq.toList(source);
       if(request.$==0?true:ServerBetfairsSession.hasNot())
        {
         _=Concurrency.Return(null);
        }
       else
        {
         x1=AjaxRemotingProvider.Async("WebFace:3",[request]);
         _=Concurrency.Bind(x1,function(_arg1)
         {
          var a;
          a=Concurrency.For(_arg1,function(_arg2)
          {
           var gameId,country;
           _arg2[1];
           gameId=_arg2[0];
           country=_arg2[2];
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
           return Concurrency.For(Var1.Get(Coupon.meetups().Var),function(_arg3)
           {
            var tupledArg,_arg00_,_arg01_;
            tupledArg=_arg3.game.gameId;
            _arg00_=tupledArg[0];
            _arg01_=tupledArg[1];
            Var1.Set(_arg3.country,Coupon.tryGetCountry(_arg00_,_arg01_));
            return Concurrency.Return(null);
           });
          }));
         });
        }
       return _;
      });
     },
     processMarkets:function()
     {
      return Concurrency.Delay(function()
      {
       var predicate,source,source1,_events_;
       predicate=function(x)
       {
        return x.markets.$==0;
       };
       source=Var1.Get(Coupon.eventsCatalogue().Var);
       source1=Seq.filter(predicate,source);
       _events_=Seq.toList(source1);
       return(_events_.$==0?true:ServerBetfairsSession.hasNot())?Concurrency.Return(null):Concurrency.For(_events_,function(_arg1)
       {
        var tupledArg,_arg00_,_arg01_,x;
        tupledArg=_arg1.gameId;
        _arg00_=tupledArg[0];
        _arg01_=tupledArg[1];
        x=AjaxRemotingProvider.Async("WebFace:4",[_arg00_,_arg01_]);
        return Concurrency.Bind(x,function(_arg2)
        {
         var _,value,chooser,folder,tupledArg2,_arg00_1,_arg01_1,list,toltalMatched,mapping,markets,arg00,arg10;
         if(_arg2.$==1)
          {
           value=_arg2.$0;
           chooser=function(tupledArg1)
           {
            var x1;
            tupledArg1[0];
            tupledArg1[1];
            tupledArg1[2];
            x1=tupledArg1[3];
            return x1;
           };
           folder=function(x1)
           {
            return function(y)
            {
             return x1+y;
            };
           };
           tupledArg2=_arg1.gameId;
           _arg00_1=tupledArg2[0];
           _arg01_1=tupledArg2[1];
           list=List.choose(chooser,value);
           toltalMatched=Seq.fold(folder,0,list);
           Coupon.updateTotalMatched(_arg00_1,_arg01_1,toltalMatched);
           mapping=function(tupledArg1)
           {
            var marketId,marketName,runners,mapping1;
            marketId=tupledArg1[0];
            marketName=tupledArg1[1];
            runners=tupledArg1[2];
            tupledArg1[3];
            mapping1=function(tupledArg3)
            {
             var runnerNamem,selectionId;
             runnerNamem=tupledArg3[0];
             selectionId=tupledArg3[1];
             return{
              selectionId:selectionId,
              runnerName:runnerNamem
             };
            };
            return{
             marketId:marketId,
             marketName:marketName,
             runners:List.map(mapping1,runners)
            };
           };
           markets=List.map(mapping,value);
           arg00=function(x1)
           {
            return{
             $:1,
             $0:Runtime.New(EventCatalogue,{
              gameId:x1.gameId,
              country:x1.country,
              markets:markets
             })
            };
           };
           arg10=_arg1.gameId;
           Coupon.eventsCatalogue().UpdateBy(arg00,arg10);
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
     },
     renderGamesHeaderRow:Runtime.Field(function()
     {
      var arg20,arg201,arg202,arg203,arg204,arg205,arg206;
      arg20=List.ofArray([Doc.TextNode("\u2116")]);
      arg201=List.ofArray([Doc.TextNode("1")]);
      arg202=Runtime.New(T,{
       $:0
      });
      arg203=List.ofArray([Doc.TextNode("2")]);
      arg204=Runtime.New(T,{
       $:0
      });
      arg205=List.ofArray([Doc.TextNode("GPB")]);
      arg206=Runtime.New(T,{
       $:0
      });
      return List.ofArray([Doc.Element("th",[],arg20),Doc.Element("th",[],arg201),Doc.Element("th",[],arg202),Doc.Element("th",[],arg203),Doc.Element("th",[],arg204),Doc.Element("th",List.ofArray([AttrProxy.Create("colspan","2")]),List.ofArray([Doc.TextNode("1")])),Doc.Element("th",List.ofArray([AttrProxy.Create("colspan","2")]),List.ofArray([Doc.TextNode("×")])),Doc.Element("th",List.ofArray([AttrProxy.Create("colspan","2")]),List.ofArray([Doc.TextNode("2")])),Doc.Element("th",[],arg205),Doc.Element("th",[],arg206)]);
     }),
     renderMeetup:function(x)
     {
      var _kef_,_bck_,_lay_,ch,arg20,arg001,arg101,v2,x2,x3,_builder_,x4,_arg00_,xa,xb,xc,arg002,_arg00_1,xd;
      _kef_=function(back)
      {
       return function(v)
       {
        var x1,arg00,arg10,v1;
        arg00=function(_arg1)
        {
         return Utils.formatDecimalOption(_arg1);
        };
        arg10=v.get_View();
        v1=View.Map(arg00,arg10);
        x1=Doc.Element("td",List.ofArray([AttrProxy.Create("class",back?"kef kef-back":"kef kef-lay")]),List.ofArray([Doc.TextView(v1)]));
        return Coupon.doc(x1);
       };
      };
      _bck_=_kef_(true);
      _lay_=_kef_(false);
      arg001=function(tupledArg)
      {
       var page,n;
       page=tupledArg[0];
       n=tupledArg[1];
       return Global.String(page)+"."+Global.String(n);
      };
      arg101=x.order.get_View();
      v2=View.Map(arg001,arg101);
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
            _1=Coupon.doc(x5);
           }
          else
           {
            if(_arg2!=="")
             {
              x6=Doc.Element("td",List.ofArray([AttrProxy.Create("class","game-status")]),List.ofArray([Doc.TextNode(_arg2)]));
              _2=Coupon.doc(x6);
             }
            else
             {
              arg201=Runtime.New(T,{
               $:0
              });
              x7=Doc.Element("td",[],arg201);
              _2=Coupon.doc(x7);
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
            _3=Coupon.doc(x8);
           }
          else
           {
            arg202=Runtime.New(T,{
             $:0
            });
            x9=Doc.Element("td",[],arg202);
            _3=Coupon.doc(x9);
           }
          _=_3;
         }
        return View1.Const(_);
       },x1);
      },x4);
      xa=Doc.Element("td",List.ofArray([AttrProxy.Create("class","away-team")]),List.ofArray([Doc.TextNode(x.game.away)]));
      xb=Doc.Element("td",List.ofArray([AttrProxy.Create("class","game-status")]),List.ofArray([Doc.TextView(x.status.get_View())]));
      xc=x.totalMatched.get_View();
      arg002=function(_arg3)
      {
       var _,totalMatched,t,arg201;
       if(_arg3.$==1)
        {
         totalMatched=_arg3.$0;
         t=Global.String(totalMatched);
         _=Doc.Element("td",List.ofArray([AttrProxy.Create("class","game-gpb")]),List.ofArray([Doc.TextNode(t)]));
        }
       else
        {
         arg201=Runtime.New(T,{
          $:0
         });
         _=Doc.Element("td",[],arg201);
        }
       return _;
      };
      _arg00_1=View.Map(arg002,xc);
      xd=Doc.Element("td",List.ofArray([AttrProxy.Create("class","game-country")]),List.ofArray([Doc.TextView(x.country.get_View())]));
      ch=List.ofArray([Coupon.doc(x2),Coupon.doc(x3),Doc.EmbedView(_arg00_),Coupon.doc(xa),Coupon.doc(xb),_bck_(x.winBack),_lay_(x.winLay),_bck_(x.drawBack),_lay_(x.drawLay),_bck_(x.loseBack),_lay_(x.loseLay),Doc.EmbedView(_arg00_1),Coupon.doc(xd)]);
      return Doc.Element("tr",[],ch);
     },
     renderPagination:Runtime.Field(function()
     {
      var _builder_,x,_arg00_;
      _builder_=View.get_Do();
      x=Coupon.varPagesCount().get_View();
      _arg00_=View1.Bind(function(_arg1)
      {
       var _,x1;
       if(_arg1<2)
        {
         _=View1.Const(Doc.get_Empty());
        }
       else
        {
         x1=Coupon.varCurrentPageNumber().get_View();
         _=View1.Bind(function(_arg2)
         {
          var arg00;
          arg00=Seq.toList(Seq.delay(function()
          {
           return Seq.map(function(n)
           {
            var aattrs,t,v,arg20,x3;
            aattrs=Seq.toList(Seq.delay(function()
            {
             return Seq.append([AttrProxy.Create("href","#")],Seq.delay(function()
             {
              return Seq.append([AttrModule.Handler("click",function()
              {
               return function()
               {
                return Var1.Set(Coupon.varCurrentPageNumber(),n);
               };
              })],Seq.delay(function()
              {
               return n===_arg2?[AttrProxy.Create("class","w3-green")]:Seq.empty();
              }));
             }));
            }));
            t="\u0421\u0442\u0440\u0430\u043d\u0438\u0446\u0430 "+Global.String(n+1);
            v=Doc.TextNode(t);
            arg20=List.ofArray([Doc.Element("a",aattrs,List.ofArray([v]))]);
            x3=Doc.Element("li",[],arg20);
            return Coupon.doc(x3);
           },Operators.range(0,_arg1));
          }));
          return View1.Const(Doc.Concat(arg00));
         },x1);
        }
       return _;
      },x);
      return Doc.EmbedView(_arg00_);
     }),
     "render\u0421oupon":Runtime.Field(function()
     {
      var ats,arg20,arg201,arg202,mapping,arg00,arg10,_arg00_;
      ats=List.ofArray([AttrProxy.Create("class","w3-container")]);
      arg201=List.ofArray([Doc.Element("tr",List.ofArray([AttrModule.Class("coupon-header-row")]),Seq.map(function(x)
      {
       return Coupon.doc(x);
      },Coupon.renderGamesHeaderRow()))]);
      mapping=function(x)
      {
       var x1;
       x1=Coupon.renderMeetup(x);
       return Coupon.doc(x1);
      };
      arg00=function(x)
      {
       var arg001;
       arg001=Seq.map(mapping,x);
       return Doc.Concat(arg001);
      };
      arg10=Coupon.meetups().get_View();
      _arg00_=View.Map(arg00,arg10);
      arg202=List.ofArray([Doc.EmbedView(_arg00_)]);
      arg20=List.ofArray([Doc.Element("thead",[],arg201),Doc.Element("tbody",[],arg202)]);
      return Doc.Element("div",ats,List.ofArray([Doc.Element("div",List.ofArray([AttrProxy.Create("class","w3-center")]),List.ofArray([Doc.Element("ul",List.ofArray([AttrProxy.Create("class","w3-pagination")]),List.ofArray([Coupon.renderPagination()]))])),Doc.Element("table",[],arg20)]));
     }),
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
     updateCoupon:function(newGms,updGms,outGms)
     {
      var value,_,clo1,arg10,value1,_2,action,clo11,arg101,action1;
      value=newGms.$==0;
      if(!value)
       {
        clo1=function(_1)
        {
         var s;
         s="adding new games "+Global.String(_1);
         return console?console.log(s):undefined;
        };
        arg10=newGms.get_Length();
        clo1(arg10);
        _=Coupon.addNewGames(newGms);
       }
      else
       {
        _=null;
       }
      value1=Seq.isEmpty(outGms);
      if(!value1)
       {
        action=function(arg00)
        {
         return Coupon.meetups().RemoveByKey(arg00);
        };
        Seq.iter(action,outGms);
        clo11=function(_1)
        {
         var s;
         s="removing out games "+Global.String(_1);
         return console?console.log(s):undefined;
        };
        arg101=Seq.length(outGms);
        _2=clo11(arg101);
       }
      else
       {
        _2=null;
       }
      action1=function(tupledArg)
      {
       var gameId,i,hash,matchValue,_1,x,_3,x1;
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
           _3=Coupon.stvr(x1.loseLay,i.loseLay);
          }
         else
          {
           _3=null;
          }
         _1=_3;
        }
       else
        {
         _1=null;
        }
       return _1;
      };
      return Seq.iter(action1,updGms);
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
     varCurrentPageNumber:Runtime.Field(function()
     {
      return Var.Create(0);
     }),
     varDataRecived:Runtime.Field(function()
     {
      return Var.Create(false);
     }),
     varPagesCount:Runtime.Field(function()
     {
      return Var.Create(1);
     })
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
              _5=Operators.Raise(MatchFailureException.New("E:\\User\\Docs\\Visual Studio 2015\\Projects\\Betfair\\CentBet\\WebFace\\ClientUtils.fs",8,18));
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
  List=Runtime.Safe(Global.WebSharper.List);
  CentBet=Runtime.Safe(Global.CentBet);
  Client=Runtime.Safe(CentBet.Client);
  Admin=Runtime.Safe(Client.Admin);
  T=Runtime.Safe(List.T);
  UI=Runtime.Safe(Global.WebSharper.UI);
  Next=Runtime.Safe(UI.Next);
  Doc=Runtime.Safe(Next.Doc);
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
  Storage1=Runtime.Safe(Next.Storage1);
  Json=Runtime.Safe(Global.WebSharper.Json);
  Provider=Runtime.Safe(Json.Provider);
  Id=Runtime.Safe(Provider.Id);
  ListModel=Runtime.Safe(Next.ListModel);
  Coupon=Runtime.Safe(Client.Coupon);
  ServerBetfairsSession=Runtime.Safe(Coupon.ServerBetfairsSession);
  Work=Runtime.Safe(Coupon.Work);
  PrintfHelpers=Runtime.Safe(Global.WebSharper.PrintfHelpers);
  console=Runtime.Safe(Global.console);
  Meetup=Runtime.Safe(Coupon.Meetup);
  Utils=Runtime.Safe(Client.Utils);
  LocalStorage=Runtime.Safe(Utils.LocalStorage);
  EventCatalogue=Runtime.Safe(Coupon.EventCatalogue);
  View1=Runtime.Safe(Next.View1);
  Operators=Runtime.Safe(Global.WebSharper.Operators);
  Date=Runtime.Safe(Global.Date);
  JSON=Runtime.Safe(Global.JSON);
  window=Runtime.Safe(Global.window);
  Collections=Runtime.Safe(Global.WebSharper.Collections);
  MapModule=Runtime.Safe(Collections.MapModule);
  FSharpSet=Runtime.Safe(Collections.FSharpSet);
  BalancedTree=Runtime.Safe(Collections.BalancedTree);
  Slice=Runtime.Safe(Global.WebSharper.Slice);
  return MatchFailureException=Runtime.Safe(Global.WebSharper.MatchFailureException);
 });
 Runtime.OnLoad(function()
 {
  LocalStorage.strg();
  Coupon.varPagesCount();
  Coupon.varDataRecived();
  Coupon.varCurrentPageNumber();
  Coupon["render\u0421oupon"]();
  Coupon.renderPagination();
  Coupon.renderGamesHeaderRow();
  Coupon.meetups();
  Coupon.eventsCatalogue();
  ServerBetfairsSession.hasServerBetfairsSession();
  Admin.varConsole();
  Admin.varCommandsHistory();
  Admin.varCmd();
  Admin.renderRecord();
  Admin.op_SpliceUntyped();
  Admin["cmd-input"]();
  return;
 });
}());

//# sourceMappingURL=WebFace.map