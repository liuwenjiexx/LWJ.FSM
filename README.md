# LWJ.FSM
Xml FSM (Finite State Machine)



### 1. ����
---
* initial
 ָ����ʼ��״̬
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
 ת������һ��״̬
 
 ```
 ���� to.s2 �¼���ת����s2״̬
<fsm initial="s1">
  <state name="s1">
    <transition target="s2" event="to.s2"/>    
  </state>
  <state name="s2"></state>
</fsm>
 ```

* param
 �������
 ```
 <state>
  <params>
      <param name="int32ByValue"  type="int32"/>
      <param name="stringByExpr"  type="string"/>
  </params>
  <state>
 ```
 

### 2. �¼� Events
---
* onEntry
 ����״̬
```
<state>
   <onEntry>
   [actions...]
   </onEntry>
</state>
```
* onExit
 �뿪״̬
```
<state>
   <onExit>
   [actions...]
   </onExit>
</state>
```

### 3. ���� Actions
---

 * log
  �����־
 ```
<onEntry>
      <log msg="log message"/>
</onEntry>
 ```
* assign

 ���ñ���ֵ
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
 
