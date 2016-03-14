(function()
{
 var Global=this,Runtime=this.IntelliFactory.Runtime,JSON,PrintfHelpers,window,UI,Next,Doc,List,CentBet,Client,Admin,T,AttrModule,AttrProxy,Seq,Key,Var,Concurrency,Var1,Option,View,Level,Strings,Seq1,Remoting,AjaxRemotingProvider,Unchecked,Storage1,Json,Provider,Id,ListModel,Coupon,View1,Meetup,Event,Utils,console,Collections,MapModule,Date,FSharpSet,BalancedTree,Slice,Operators,MatchFailureException;
 Runtime.Define(Global,{
  CentBet:{
   Client:{
    Admin:{
     Level:Runtime.Class({},{
      get_color:function()
      {
       return function(_arg1)
       {
        return _arg1.$==1?["lightgrey","red"]:_arg1.$==2?["lightgrey","green"]:["white","navy"];
       };
      }
     }),
     LocalStorage:Runtime.Class({
      Get:function(k)
      {
       var _,arg00,value,e,_1;
       try
       {
        arg00=this.strg.getItem(k);
        value=JSON.parse(arg00);
        _={
         $:0,
         $0:value
        };
       }
       catch(e)
       {
        _1=this.strg;
        _={
         $:1,
         $0:"local storage get item "+PrintfHelpers.prettyPrint(k)+" error : "+PrintfHelpers.prettyPrint(e)+", storage "+PrintfHelpers.prettyPrint(_1)
        };
       }
       return _;
      },
      Set:function(k,value)
      {
       var _,value1,e,_1;
       try
       {
        value1=JSON.stringify(value);
        _={
         $:0,
         $0:this.strg.setItem(k,value1)
        };
       }
       catch(e)
       {
        _1=this.strg;
        _={
         $:1,
         $0:"local storage set item "+PrintfHelpers.prettyPrint(k)+" error : "+PrintfHelpers.prettyPrint(e)+", value "+PrintfHelpers.prettyPrint(value)+", storage "+PrintfHelpers.prettyPrint(_1)
        };
       }
       return _;
      }
     },{
      New:function()
      {
       var r;
       r=Runtime.New(this,{});
       r.strg=window.localStorage;
       return r;
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
     RenderMenu:function()
     {
      var mapping,list,arg00;
      mapping=function(x)
      {
       return x;
      };
      list=List.ofArray([Doc.Element("a",Seq.toList(Seq.delay(function()
      {
       return Seq.append([AttrProxy.Create("href","#")],Seq.delay(function()
       {
        return[AttrModule.Handler("click",function()
        {
         return function()
         {
          return Admin.varConsole().Clear();
         };
        })];
       }));
      })),List.ofArray([Doc.TextNode("Clear console")])),Doc.Element("a",Seq.toList(Seq.delay(function()
      {
       return Seq.append([AttrProxy.Create("href","#")],Seq.delay(function()
       {
        return[AttrModule.Handler("click",function()
        {
         return function()
         {
          return Admin.varCommandsHistory().Clear();
         };
        })];
       }));
      })),List.ofArray([Doc.TextNode("Clear history")]))]);
      arg00=List.map(mapping,list);
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
     addRecord:function(level,text)
     {
      return Admin.varConsole().Add({
       Id:Key.Fresh(),
       Text:text,
       Level:level
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
              var key;
              key=e.keyCode;
              return key===13?Concurrency.Start(doSend,{
               $:0
              }):key===38?setCommandFromHistory(true):key===40?setCommandFromHistory(false):null;
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
       patternInput=(Level.get_color())(r.Level);
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
       Admin.addRecord(Runtime.New(Level,{
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
         return Concurrency.Bind(AjaxRemotingProvider.Async("WebFace:1",[inputText1]),function(_arg1)
         {
          var msg;
          _arg1[0];
          msg=_arg1[1];
          Admin.addRecord(Runtime.New(Level,{
           $:0
          }),msg);
          return Concurrency.Return(null);
         });
        }),function(_arg2)
        {
         Admin.addRecord(Runtime.New(Level,{
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
      var arg00,arg10;
      arg00=function(x)
      {
       return Admin.cmdKey(x);
      };
      arg10=Storage1.LocalStorage("CentBetConsoleCommandsHistory",{
       Encode:(Provider.get_Default().EncodeRecord(undefined,[["Id",Provider.get_Default().EncodeUnion(Key,"$",[[0,[["$0","Item",Id,0]]]]),0],["Text",Id,0]]))(),
       Decode:(Provider.get_Default().DecodeRecord(undefined,[["Id",Provider.get_Default().DecodeUnion(Key,"$",[[0,[["$0","Item",Id,0]]]]),0],["Text",Id,0]]))()
      });
      return ListModel.CreateWithStorage(arg00,arg10);
     }),
     varConsole:Runtime.Field(function()
     {
      var arg00,arg10;
      arg00=function(x)
      {
       return Admin.recordKey(x);
      };
      arg10=Storage1.LocalStorage("CentBetConsole",{
       Encode:(Provider.get_Default().EncodeRecord(undefined,[["Id",Provider.get_Default().EncodeUnion(Key,"$",[[0,[["$0","Item",Id,0]]]]),0],["Text",Id,0],["Level",Provider.get_Default().EncodeUnion(Level,"$",[[0,[]],[1,[]],[2,[]]]),0]]))(),
       Decode:(Provider.get_Default().DecodeRecord(undefined,[["Id",Provider.get_Default().DecodeUnion(Key,"$",[[0,[["$0","Item",Id,0]]]]),0],["Text",Id,0],["Level",Provider.get_Default().DecodeUnion(Level,"$",[[0,[]],[1,[]],[2,[]]]),0]]))()
      });
      return ListModel.CreateWithStorage(arg00,arg10);
     })
    },
    Coupon:{
     Event:Runtime.Class({},{
      id:function(x)
      {
       return x.gameId;
      }
     }),
     Meetup:Runtime.Class({},{
      id:function(x)
      {
       return x.game.gameId;
      },
      inplay:function(x)
      {
       return Var1.Get(x.gameInfo).playMinute.$==1;
      },
      notinplay:function(x)
      {
       return Var1.Get(x.gameInfo).playMinute.$==0;
      },
      viewGameInfo:function(x)
      {
       return x.gameInfo.get_View();
      }
     }),
     Render:function()
     {
      var _builder_,x,x1,arg00,x2;
      Concurrency.Start(Coupon.workloop(),{
       $:0
      });
      _builder_=View.get_Do();
      x=Coupon.varInplayOnly().get_View();
      x1=View1.Bind(function(_arg1)
      {
       return View1.Bind(function(_arg2)
       {
        return View1.Bind(function(_arg3)
        {
         var value,hasgames;
         value=Seq.isEmpty(_arg3);
         hasgames=!value;
         return View1.Const([hasgames,_arg1,_arg2]);
        },Coupon.meetups().get_View());
       },Coupon.varDataRecived().get_View());
      },x);
      arg00=function(_arg4)
      {
       var _,_1,arg20,_2,arg201,arg202,arg203;
       if(_arg4[2])
        {
         if(_arg4[0])
          {
           arg20=List.ofArray([Coupon.renderMeetups()]);
           _1=Doc.Element("table",Runtime.New(T,{
            $:0
           }),List.ofArray([Doc.Element("tbody",[],arg20)]));
          }
         else
          {
           if(_arg4[1])
            {
             arg201=List.ofArray([Doc.TextNode("\u041d\u0435\u0442 \u0434\u0430\u043d\u043d\u044b\u0445 \u043e \u0440\u0430\u0437\u044b\u0433\u0440\u044b\u0432\u0430\u0435\u043c\u044b\u0445 \u0432 \u043d\u0430\u0441\u0442\u043e\u044f\u0449\u0438\u0439 \u043c\u043e\u043c\u0435\u043d\u0442 \u0444\u0443\u0442\u0431\u043e\u043b\u044c\u043d\u044b\u0445 \u043c\u0430\u0442\u0447\u0430\u0445")]);
             _2=Doc.Element("h1",[],arg201);
            }
           else
            {
             arg202=List.ofArray([Doc.TextNode("\u041d\u0435\u0442 \u0434\u0430\u043d\u043d\u044b\u0445 \u043e \u0444\u0443\u0442\u0431\u043e\u043b\u044c\u043d\u044b\u0445 \u043c\u0430\u0442\u0447\u0430\u0445 \u043d\u0430 \u0441\u0435\u0433\u043e\u0434\u043d\u044f")]);
             _2=Doc.Element("h1",[],arg202);
            }
           _1=_2;
          }
         _=_1;
        }
       else
        {
         arg203=List.ofArray([Doc.TextNode("\u0414\u0430\u043d\u043d\u044b\u0435 \u0437\u0430\u0433\u0440\u0443\u0436\u0430\u044e\u0442\u0441\u044f \u0441 \u0441\u0435\u0440\u0432\u0435\u0440\u0430. \u041f\u043e\u0436\u0430\u043b\u0443\u0439\u0441\u0442\u0430, \u043f\u043e\u0434\u043e\u0436\u0434\u0438\u0442\u0435.")]);
         _=Doc.Element("h1",[],arg203);
        }
       return _;
      };
      x2=View.Map(arg00,x1);
      return Doc.EmbedView(x2);
     },
     RenderMenu:function()
     {
      var arg00;
      arg00=List.ofArray([Coupon.renderMenusInplay(),Coupon.renderMenuCountries()]);
      return Doc.Concat(arg00);
     },
     addNewGames:function(newGames)
     {
      var source,existedMeetups,mapping,projection,action,source2,source1,source3;
      source=Var1.Get(Coupon.meetups().Var);
      existedMeetups=Seq.toList(source);
      Coupon.meetups().Clear();
      mapping=function(tupledArg)
      {
       var game,gameInfo,hash,tupledArg1,_arg00_,_arg01_,country,gameInfo1;
       game=tupledArg[0];
       gameInfo=tupledArg[1];
       hash=tupledArg[2];
       tupledArg1=game.gameId;
       _arg00_=tupledArg1[0];
       _arg01_=tupledArg1[1];
       country=Coupon.tryGetCountry(_arg00_,_arg01_);
       gameInfo1=Var.Create(gameInfo);
       return Runtime.New(Meetup,{
        game:game,
        gameInfo:gameInfo1,
        country:Var.Create(country),
        hash:hash
       });
      };
      projection=function(x)
      {
       return Var1.Get(x.gameInfo).order;
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
     events:Runtime.Field(function()
     {
      var arg00,arg10;
      arg00=function(arg001)
      {
       return Event.id(arg001);
      };
      arg10=Runtime.New(T,{
       $:0
      });
      return ListModel.Create(arg00,arg10);
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
       var mapping,source,source1,request;
       mapping=function(m)
       {
        return[m.game.gameId,m.hash];
       };
       source=Var1.Get(Coupon.meetups().Var);
       source1=Seq.map(mapping,source);
       request=Seq.toList(source1);
       return Concurrency.Bind(AjaxRemotingProvider.Async("WebFace:0",[request,Var1.Get(Coupon.varInplayOnly())]),function(_arg1)
       {
        var updGms,outGms,newGms;
        updGms=_arg1[1];
        outGms=_arg1[2];
        newGms=_arg1[0];
        Coupon.updateCoupon(newGms,updGms,outGms);
        return Concurrency.Return(null);
       });
      });
     },
     processEvents:function()
     {
      return Concurrency.Delay(function()
      {
       var _events_,x,chooser,source,request,_,x1;
       _events_=Var1.Get(Coupon.events().Var);
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
       if(request.$==0)
        {
         _=Concurrency.Return(null);
        }
       else
        {
         x1=AjaxRemotingProvider.Async("WebFace:2",[request]);
         _=Concurrency.Bind(x1,function(_arg1)
         {
          var a;
          a=Concurrency.For(_arg1,function(_arg2)
          {
           var openDate,gameId,country;
           openDate=_arg2[3];
           _arg2[1];
           gameId=_arg2[0];
           country=_arg2[2];
           Coupon.events().Add(Runtime.New(Event,{
            gameId:gameId,
            country:country,
            openDate:openDate
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
     renderMeetup:function(x,inplayOnly,selectedCountry)
     {
      var matchValue,_,_1,_2,selectedCountry1,_3,x1,arg00,_arg00_,x3,arg001,_arg00_1,_5,selectedCountry2,_6,x4,arg002,_arg00_2,x5,arg003,_arg00_3;
      matchValue=[inplayOnly,Var1.Get(x.gameInfo).playMinute,selectedCountry];
      if(matchValue[0])
       {
        if(matchValue[1].$==0)
         {
          _1=Doc.get_Empty();
         }
        else
         {
          if(matchValue[2].$==1)
           {
            selectedCountry1=matchValue[2].$0;
            if(selectedCountry1!==""?selectedCountry1!==Var1.Get(x.country):false)
             {
              matchValue[2].$0;
              _3=Doc.get_Empty();
             }
            else
             {
              x1=x.gameInfo.get_View();
              arg00=function(i)
              {
               var predicate,list,_4,mapping,list1,ch;
               predicate=function(option)
               {
                return option.$==1;
               };
               list=List.ofArray([i.drawBack,i.drawLay,i.winBack,i.winLay,i.loseBack,i.loseLay]);
               if(Seq.exists(predicate,list))
                {
                 mapping=function(x2)
                 {
                  return x2;
                 };
                 list1=Coupon.renderMeetup1(x,selectedCountry.$==1);
                 ch=List.map(mapping,list1);
                 _4=Doc.Element("tr",[],ch);
                }
               else
                {
                 _4=Doc.get_Empty();
                }
               return _4;
              };
              _arg00_=View.Map(arg00,x1);
              _3=Doc.EmbedView(_arg00_);
             }
            _2=_3;
           }
          else
           {
            x3=x.gameInfo.get_View();
            arg001=function(i)
            {
             var predicate,list,_4,mapping,list1,ch;
             predicate=function(option)
             {
              return option.$==1;
             };
             list=List.ofArray([i.drawBack,i.drawLay,i.winBack,i.winLay,i.loseBack,i.loseLay]);
             if(Seq.exists(predicate,list))
              {
               mapping=function(x2)
               {
                return x2;
               };
               list1=Coupon.renderMeetup1(x,selectedCountry.$==1);
               ch=List.map(mapping,list1);
               _4=Doc.Element("tr",[],ch);
              }
             else
              {
               _4=Doc.get_Empty();
              }
             return _4;
            };
            _arg00_1=View.Map(arg001,x3);
            _2=Doc.EmbedView(_arg00_1);
           }
          _1=_2;
         }
        _=_1;
       }
      else
       {
        if(matchValue[2].$==1)
         {
          selectedCountry2=matchValue[2].$0;
          if(selectedCountry2!==""?selectedCountry2!==Var1.Get(x.country):false)
           {
            matchValue[2].$0;
            _6=Doc.get_Empty();
           }
          else
           {
            x4=x.gameInfo.get_View();
            arg002=function(i)
            {
             var predicate,list,_4,mapping,list1,ch;
             predicate=function(option)
             {
              return option.$==1;
             };
             list=List.ofArray([i.drawBack,i.drawLay,i.winBack,i.winLay,i.loseBack,i.loseLay]);
             if(Seq.exists(predicate,list))
              {
               mapping=function(x2)
               {
                return x2;
               };
               list1=Coupon.renderMeetup1(x,selectedCountry.$==1);
               ch=List.map(mapping,list1);
               _4=Doc.Element("tr",[],ch);
              }
             else
              {
               _4=Doc.get_Empty();
              }
             return _4;
            };
            _arg00_2=View.Map(arg002,x4);
            _6=Doc.EmbedView(_arg00_2);
           }
          _5=_6;
         }
        else
         {
          x5=x.gameInfo.get_View();
          arg003=function(i)
          {
           var predicate,list,_4,mapping,list1,ch;
           predicate=function(option)
           {
            return option.$==1;
           };
           list=List.ofArray([i.drawBack,i.drawLay,i.winBack,i.winLay,i.loseBack,i.loseLay]);
           if(Seq.exists(predicate,list))
            {
             mapping=function(x2)
             {
              return x2;
             };
             list1=Coupon.renderMeetup1(x,selectedCountry.$==1);
             ch=List.map(mapping,list1);
             _4=Doc.Element("tr",[],ch);
            }
           else
            {
             _4=Doc.get_Empty();
            }
           return _4;
          };
          _arg00_3=View.Map(arg003,x5);
          _5=Doc.EmbedView(_arg00_3);
         }
        _=_5;
       }
      return _;
     },
     renderMeetup1:function(x,countryIsSelected)
     {
      var vinfo,_kef_,op_SpliceUntyped;
      vinfo=function(f)
      {
       var arg10,_arg00_;
       arg10=x.gameInfo.get_View();
       _arg00_=View.Map(f,arg10);
       return Doc.TextView(_arg00_);
      };
      _kef_=function(back,f)
      {
       return Doc.Element("td",List.ofArray([AttrProxy.Create("class",back?"kef kef-back":"kef kef-lay")]),List.ofArray([vinfo(f)]));
      };
      op_SpliceUntyped=function(_arg1)
      {
       return Utils.formatDecimalOption(_arg1);
      };
      return Seq.toList(Seq.delay(function()
      {
       var arg20;
       arg20=List.ofArray([vinfo(function(y)
       {
        var patternInput,page,n;
        patternInput=y.order;
        page=patternInput[0];
        n=patternInput[1];
        return Global.String(page)+"."+Global.String(n);
       })]);
       return Seq.append([Doc.Element("td",[],arg20)],Seq.delay(function()
       {
        return Seq.append([Doc.Element("td",List.ofArray([AttrProxy.Create("class","home-team")]),List.ofArray([Doc.TextNode(x.game.home)]))],Seq.delay(function()
        {
         return Seq.append([Doc.Element("td",List.ofArray([AttrProxy.Create("class","game-status")]),List.ofArray([vinfo(function(y)
         {
          return y.summary;
         })]))],Seq.delay(function()
         {
          return Seq.append([Doc.Element("td",List.ofArray([AttrProxy.Create("class","away-team")]),List.ofArray([Doc.TextNode(x.game.away)]))],Seq.delay(function()
          {
           return Seq.append([Doc.Element("td",List.ofArray([AttrProxy.Create("class","game-status")]),List.ofArray([vinfo(function(y)
           {
            return y.status;
           })]))],Seq.delay(function()
           {
            return Seq.append([_kef_(true,function(y)
            {
             return op_SpliceUntyped(y.winBack);
            })],Seq.delay(function()
            {
             return Seq.append([_kef_(false,function(y)
             {
              return op_SpliceUntyped(y.winLay);
             })],Seq.delay(function()
             {
              return Seq.append([_kef_(true,function(y)
              {
               return op_SpliceUntyped(y.drawBack);
              })],Seq.delay(function()
              {
               return Seq.append([_kef_(false,function(y)
               {
                return op_SpliceUntyped(y.drawLay);
               })],Seq.delay(function()
               {
                return Seq.append([_kef_(true,function(y)
                {
                 return op_SpliceUntyped(y.loseBack);
                })],Seq.delay(function()
                {
                 return Seq.append([_kef_(false,function(y)
                 {
                  return op_SpliceUntyped(y.loseLay);
                 })],Seq.delay(function()
                 {
                  var _,arg201,x1;
                  if(!countryIsSelected)
                   {
                    x1=Doc.TextView(x.country.get_View());
                    arg201=List.ofArray([Doc.Element("span",List.ofArray([AttrProxy.Create("class","game-status")]),List.ofArray([x1]))]);
                    _=[Doc.Element("td",[],arg201)];
                   }
                  else
                   {
                    _=Seq.empty();
                   }
                  return _;
                 }));
                }));
               }));
              }));
             }));
            }));
           }));
          }));
         }));
        }));
       }));
      }));
     },
     renderMeetups:function()
     {
      var x,_arg00_;
      x=Coupon.meetups().get_View();
      _arg00_=function(x1)
      {
       var arg00,arg10,_arg00_1;
       arg00=function(tupledArg)
       {
        var x2,inplayOnly,selectedCountry;
        x2=tupledArg[0];
        inplayOnly=tupledArg[1];
        selectedCountry=tupledArg[2];
        return Coupon.renderMeetup(x2,inplayOnly,selectedCountry);
       };
       View.get_Do();
       arg10=View1.Bind(function(_arg1)
       {
        return View1.Bind(function(_arg2)
        {
         return View1.Bind(function(_arg3)
         {
          return View1.Const([_arg1,_arg2,_arg3]);
         },Coupon.varSelectedCountry().get_View());
        },Coupon.varInplayOnly().get_View());
       },x1);
       _arg00_1=View.Map(arg00,arg10);
       return Doc.EmbedView(_arg00_1);
      };
      return Doc.ConvertSeq(_arg00_,x);
     },
     renderMenuCountries:function()
     {
      var x,arg00,_arg00_;
      View.get_Do();
      x=View1.Bind(function(_arg1)
      {
       return View1.Bind(function(_arg2)
       {
        return View1.Const([_arg1,_arg2]);
       },Coupon.varSelectedCountry().get_View());
      },Coupon.viewCountries());
      arg00=function(tupledArg)
      {
       var countries,selectedCountry,_,mapping,arg001,x1,arg002;
       countries=tupledArg[0];
       selectedCountry=tupledArg[1];
       if(Seq.isEmpty(countries))
        {
         _=Doc.get_Empty();
        }
       else
        {
         mapping=function(country)
         {
          return Coupon.renderMenuItemCountry(selectedCountry,country);
         };
         arg001=Seq.map(mapping,countries);
         x1=Doc.Concat(arg001);
         arg002=Coupon.renderMenuItemAllCountries();
         _=Doc.Append(arg002,x1);
        }
       return _;
      };
      _arg00_=View.Map(arg00,x);
      return Doc.EmbedView(_arg00_);
     },
     renderMenuItemAllCountries:function()
     {
      var x,arg00,_arg00_;
      x=Coupon.varSelectedCountry().get_View();
      arg00=function(selectedCountry)
      {
       return Doc.Element("a",Seq.toList(Seq.delay(function()
       {
        var _,selectedCountry1,_1;
        if(selectedCountry.$==1)
         {
          selectedCountry1=selectedCountry.$0;
          if(selectedCountry1!=="")
           {
            selectedCountry.$0;
            _1=Seq.empty();
           }
          else
           {
            _1=[AttrProxy.Create("class","active")];
           }
          _=_1;
         }
        else
         {
          _=[AttrProxy.Create("class","active")];
         }
        return Seq.append(_,Seq.delay(function()
        {
         return Seq.append([AttrProxy.Create("href","#")],Seq.delay(function()
         {
          return[AttrModule.Handler("click",function()
          {
           return function()
           {
            return Var1.Set(Coupon.varSelectedCountry(),{
             $:0
            });
           };
          })];
         }));
        }));
       })),List.ofArray([Doc.TextNode("\u0412\u0441\u0435 \u0441\u0442\u0440\u0430\u043d\u044b")]));
      };
      _arg00_=View.Map(arg00,x);
      return Doc.EmbedView(_arg00_);
     },
     renderMenuItemCountry:function(selectedCountry,country)
     {
      return Doc.Element("a",Seq.toList(Seq.delay(function()
      {
       var _,selectedCountry1,_1;
       if(selectedCountry.$==1)
        {
         selectedCountry1=selectedCountry.$0;
         if(selectedCountry1===country)
          {
           selectedCountry.$0;
           _1=[AttrProxy.Create("class","active")];
          }
         else
          {
           _1=Seq.empty();
          }
         _=_1;
        }
       else
        {
         _=Seq.empty();
        }
       return Seq.append(_,Seq.delay(function()
       {
        return Seq.append([AttrProxy.Create("href","#")],Seq.delay(function()
        {
         return[AttrModule.Handler("click",function()
         {
          return function()
          {
           return Var1.Set(Coupon.varSelectedCountry(),{
            $:1,
            $0:country
           });
          };
         })];
        }));
       }));
      })),List.ofArray([Doc.TextNode(country)]));
     },
     renderMenusInplay:function()
     {
      var x,arg00,_arg00_;
      x=Coupon.varInplayOnly().get_View();
      arg00=function(isInplayOnly)
      {
       var mapping,list,arg001;
       mapping=function(x1)
       {
        return x1;
       };
       list=List.ofArray([Doc.Element("a",Seq.toList(Seq.delay(function()
       {
        return Seq.append([AttrProxy.Create("href","#")],Seq.delay(function()
        {
         return Seq.append([AttrModule.Handler("click",function()
         {
          return function()
          {
           Var1.Set(Coupon.varSelectedCountry(),{
            $:0
           });
           Var1.Set(Coupon.varInplayOnly(),true);
           return Var1.Set(Coupon.varDataRecived(),false);
          };
         })],Seq.delay(function()
         {
          return isInplayOnly?[AttrProxy.Create("class","active")]:Seq.empty();
         }));
        }));
       })),List.ofArray([Doc.TextNode("\u0412 \u0438\u0433\u0440\u0435")])),Doc.Element("a",Seq.toList(Seq.delay(function()
       {
        return Seq.append([AttrProxy.Create("href","#")],Seq.delay(function()
        {
         return Seq.append([AttrModule.Handler("click",function()
         {
          return function()
          {
           Var1.Set(Coupon.varSelectedCountry(),{
            $:0
           });
           Var1.Set(Coupon.varInplayOnly(),false);
           return Var1.Set(Coupon.varDataRecived(),false);
          };
         })],Seq.delay(function()
         {
          return!isInplayOnly?[AttrProxy.Create("class","active")]:Seq.empty();
         }));
        }));
       })),List.ofArray([Doc.TextNode("\u0412\u0441\u0435 \u043c\u0430\u0442\u0447\u0438")]))]);
       arg001=List.map(mapping,list);
       return Doc.Concat(arg001);
      };
      _arg00_=View.Map(arg00,x);
      return Doc.EmbedView(_arg00_);
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
      source=Var1.Get(Coupon.events().Var);
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
       var gameId,gameInfo,hash,matchValue,_1,x;
       gameId=tupledArg[0];
       gameInfo=tupledArg[1];
       hash=tupledArg[2];
       matchValue=Coupon.meetups().TryFindByKey(gameId);
       if(matchValue.$==1)
        {
         x=matchValue.$0;
         Var1.Set(x.gameInfo,gameInfo);
         _1=void(x.hash=hash);
        }
       else
        {
         _1=null;
        }
       return _1;
      };
      return Seq.iter(action1,updGms);
     },
     varDataRecived:Runtime.Field(function()
     {
      return Var.Create(false);
     }),
     varInplayOnly:Runtime.Field(function()
     {
      return Var.Create(true);
     }),
     varSelectedCountry:Runtime.Field(function()
     {
      return Var.Create({
       $:0
      });
     }),
     viewCountries:function()
     {
      var _builder_,x;
      _builder_=View.get_Do();
      x=Coupon.events().get_View();
      return View1.Bind(function(_arg1)
      {
       var mapping,elements,events,x1;
       mapping=function(evt)
       {
        return[evt.gameId,evt];
       };
       elements=Seq.map(mapping,_arg1);
       events=MapModule.OfArray(Seq.toArray(elements));
       x1=Coupon.viewGames();
       return View1.Bind(function(_arg2)
       {
        var chooser,x2,x3,predicate,x4,chooser1,x6,x7;
        chooser=function(tupledArg)
        {
         var g,mapping1,option;
         g=tupledArg[0];
         tupledArg[1];
         mapping1=function(e)
         {
          return e.country;
         };
         option=events.TryFind(g.gameId);
         return Option.map(mapping1,option);
        };
        x2=Seq.choose(chooser,_arg2);
        x3={
         $:0
        };
        predicate=function(y)
        {
         return!Unchecked.Equals(x3,y);
        };
        x4=Seq.filter(predicate,x2);
        chooser1=function(x5)
        {
         return x5;
        };
        x6=Seq.choose(chooser1,x4);
        x7=Seq1.distinct(x6);
        return View1.Const(Seq.sort(x7));
       },x1);
      },x);
     },
     viewGames:Runtime.Field(function()
     {
      var _builder_,x;
      _builder_=View.get_Do();
      x=Coupon.meetups().get_View();
      return View1.Bind(function(_arg1)
      {
       var _,x1;
       if(Seq.isEmpty(_arg1))
        {
         _=View1.Const(Seq.empty());
        }
       else
        {
         x1=Coupon.varInplayOnly().get_View();
         _=View1.Bind(function(_arg2)
         {
          var predicate,mapping,source,arg001;
          predicate=function(x2)
          {
           return!_arg2?true:Var1.Get(x2.gameInfo).playMinute.$==1;
          };
          mapping=function(x2)
          {
           var arg00,arg10;
           arg00=function(i)
           {
            return[x2.game,i];
           };
           arg10=x2.gameInfo.get_View();
           return View.Map(arg00,arg10);
          };
          source=Seq.filter(predicate,_arg1);
          arg001=Seq.map(mapping,source);
          return View1.Bind(function(_arg3)
          {
           return View1.Const(_arg3);
          },View1.Sequence(arg001));
         },x1);
        }
       return _;
      },x);
     }),
     workloop:function()
     {
      return Concurrency.Delay(function()
      {
       return Concurrency.Combine(Concurrency.TryWith(Concurrency.Delay(function()
       {
        return Concurrency.Bind(Coupon.processCoupon(),function()
        {
         return Concurrency.Bind(Coupon.processEvents(),function()
         {
          Var1.Set(Coupon.varDataRecived(),true);
          return Concurrency.Return(null);
         });
        });
       }),function(_arg3)
       {
        var clo1;
        clo1=function(_)
        {
         var s;
         s="error updating coupon : "+PrintfHelpers.prettyPrint(_);
         return console?console.log(s):undefined;
        };
        clo1(_arg3);
        return Concurrency.Bind(Concurrency.Sleep(5000),function()
        {
         return Concurrency.Return(null);
        });
       }),Concurrency.Delay(function()
       {
        return Coupon.workloop();
       }));
      });
     }
    },
    Utils:{
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
              _5=Operators.Raise(MatchFailureException.New("E:\\User\\Docs\\Visual Studio 2015\\Projects\\Betfair\\CentBet\\WebFace\\ClientUtils.fs",9,18));
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
     }
    }
   }
  }
 });
 Runtime.OnInit(function()
 {
  JSON=Runtime.Safe(Global.JSON);
  PrintfHelpers=Runtime.Safe(Global.WebSharper.PrintfHelpers);
  window=Runtime.Safe(Global.window);
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
  Seq=Runtime.Safe(Global.WebSharper.Seq);
  Key=Runtime.Safe(Next.Key);
  Var=Runtime.Safe(Next.Var);
  Concurrency=Runtime.Safe(Global.WebSharper.Concurrency);
  Var1=Runtime.Safe(Next.Var1);
  Option=Runtime.Safe(Global.WebSharper.Option);
  View=Runtime.Safe(Next.View);
  Level=Runtime.Safe(Admin.Level);
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
  View1=Runtime.Safe(Next.View1);
  Meetup=Runtime.Safe(Coupon.Meetup);
  Event=Runtime.Safe(Coupon.Event);
  Utils=Runtime.Safe(Client.Utils);
  console=Runtime.Safe(Global.console);
  Collections=Runtime.Safe(Global.WebSharper.Collections);
  MapModule=Runtime.Safe(Collections.MapModule);
  Date=Runtime.Safe(Global.Date);
  FSharpSet=Runtime.Safe(Collections.FSharpSet);
  BalancedTree=Runtime.Safe(Collections.BalancedTree);
  Slice=Runtime.Safe(Global.WebSharper.Slice);
  Operators=Runtime.Safe(Global.WebSharper.Operators);
  return MatchFailureException=Runtime.Safe(Global.WebSharper.MatchFailureException);
 });
 Runtime.OnLoad(function()
 {
  Coupon.viewGames();
  Coupon.varSelectedCountry();
  Coupon.varInplayOnly();
  Coupon.varDataRecived();
  Coupon.meetups();
  Coupon.events();
  Admin.varConsole();
  Admin.varCommandsHistory();
  Admin.varCmd();
  Admin.renderRecord();
  Admin.op_SpliceUntyped();
  Admin["cmd-input"]();
  return;
 });
}());
