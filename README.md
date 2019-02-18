# LWJ.FSM
Xml FSM (Finite State Machine)



### 1. 基础
---
* initial
 指定初始化状态
 ```
<fsm xmlns="urn:schema-lwj:fsm" 
     xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" 
      xsi:schemaLocation="urn:schema-lwj:fsm ../../../resources/lwj.fsm.xsd"
     initial="s1" >
  <state name="s1">
  </state>   
</fsm>
 ```

* state
 ```
  <fsm initial="s1">
      <state name="s1"></state>
      <state name="s2"></state>
</fsm>
 ```

* transition
 转换到另一个状态
 
 ```
 发送 to.s2 事件将转换到s2状态
<fsm initial="s1">
  <state name="s1">
    <transition target="s2" event="to.s2"/>    
  </state>
  <state name="s2"></state>
</fsm>
 ```

* param
 定义变量
 ```
 <state>
  <params>
      <param name="int32ByValue"  type="int32"/>
      <param name="stringByExpr"  type="string"/>
  </params>
  <state>
 ```
 

### 2. 事件 Events
---
* onEntry
 进入状态
```
<state>
   <onEntry>
   [actions...]
   </onEntry>
</state>
```
* onExit
 离开状态
```
<state>
   <onExit>
   [actions...]
   </onExit>
</state>
```

### 3. 动作 Actions
---

 * log
  输出日志
 ```
<onEntry>
      <log msg="log message"/>
</onEntry>
 ```
* assign

 设置变量值
 ```
<assign name="" value=""/>
<assign name="" >xml expression</assign>
 ```
 
 ```
<fsm>
  <params>
    <param name="int32ByValue"  type="int32"/>
    <param name="stringByExpr"  type="string"/>
  </params>
  <state name="s1">
    <onEntry>
      <assign name="int32ByValue" value="1"></assign>
      <assign name="stringByExpr">
        <x:string>Hello World</x:string>
      </assign>
    </onEntry>
  </state>
  <fsm>
 ```
 
