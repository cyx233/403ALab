## 人机交互课程项目：要有光



### 研究问题

智能家居场景的统一灯光控制。

未来智能家居场景中，可能有很多个灯光设备，用户如何统一调整各个设备，使场景获得舒适的光照。



### 依赖

- Unity 2019.4.15f1
- MRTK V.2.5.1
- IoThingsLab v0.5



### 项目结构

- 构建了一个场景，位于 `Assets/com.tsinghua.iotvrp/Scenes/Client.unity`。场景含有若干光源，以及书桌、电视等物体，用于测试灯光调节算法。
- 代码均位于 `Assets/com.tsinghua.iotvrp/Server/IoThingsLab/` 文件夹下，其中：
  - 手势识别的方法位于 `MyScript/GestureRecognizerItem/` 文件夹下。其中 `GestureEventData.cs` 定义了手势事件的数据结构，`GestureRecognizerItem.cs`  进行手势识别。
- `MyScript/Controller/Controller.cs` 负责转发手势和按钮事件给光源。
- `MyScript/RayCast/RayCastDetector.cs` 计算并保存与视线相交的物体位置，用于视线调节
- `Standalone/LightItem/LightItem.cs` 实现了灯光的各种调节方法





### 使用示例

- 开关灯与亮度调节

  <video src="vid/1.mp4"></video>



- 视线触发调节

  <video src="vid/4.mp4"></video>