# Bug 踩坑记录

> 本文件用于记录项目开发过程中踩过的坑,目的是面试时能清晰复述自己排查和解决问题的思路。
>
> **格式约定**:每条 bug 包含「现象 → 排查过程 → 根因 → 解决方案 → 优化/延伸」五部分。每解决一个新问题,在下面追加一条,并更新顶部目录。

---

## 目录

- [BUG-001 人物按键无移动,但日志正常输出](#bug-001)
- [BUG-002 敌人标记 mask 不显示](#bug-002)
- [BUG-003 复制多个敌人后报 NullReferenceException](#bug-003)

---

<a id="bug-001"></a>

## BUG-001 人物按键无移动,但日志正常输出

**日期**:2026-11-07

**现象**:按下 W 键,控制台正常输出 `Player is moving`,但人物原地不动。

**排查过程**:
1. 先看代码,`PlayerMove()` 里用 `AddForce` 施加移动力,日志能打印说明按键检测和逻辑都执行了;
2. 怀疑力太小(速度 `speed=2`,`AddForce` 默认 `ForceMode.Force`),但在查看截图后发现了更直接的证据;
3. 查看 Rigidbody2D 组件设置,发现 **Constraints → Freeze Position 的 X、Y 都被勾选了**,即位置被冻结。

**根因**:Rigidbody2D 的 Freeze Position X/Y 被勾选,物理系统强制锁定物体位置,无论加多大力、设多快速度,物体都不会移动。

**解决方案**:取消 Rigidbody2D 的 Freeze Position X/Y 勾选,只保留 Freeze Rotation Z(防止 2D 人物旋转乱转)。

**优化/延伸**:
- 2D 俯视角移动推荐直接设置 `rigidbody.velocity`,而不是每帧 `AddForce`,手感更稳且不受质量和 Linear Drag 影响;
- 排查"代码执行了但效果不对"时,优先检查物理/渲染层面的限制(Constraints、Layer、Trigger),往往比逻辑 bug 更快定位。

---

<a id="bug-002"></a>

## BUG-002 敌人标记 mask 不显示

**日期**:2026-11-07

**现象**:运行后敌人身上没有出现标记(mask)。

**排查过程**:
1. 定位到 `PlayerAiM.cs` 的 `Update()` 里,`mark.SetActive(true)` 位于某个 `if` 内;
2. 发现判断条件 `distance < maxDistance && distance < minDistance`,而 `Start()` 里 `minDistance = maxDistance = 0`;
3. 距离永远是正数,`distance < 0` 永远不成立,所以 `mark.SetActive(true)` 永远执行不到。

**根因**:判断条件自相矛盾。`minDistance` 和 `maxDistance` 都初始化为 0,导致 `distance < maxDistance`(即 `distance < 0`)恒为 false。

**解决方案**:用「找最近距离」的正确逻辑 —— 设一个初始为极大值的最近距离变量,遍历比较取最小值,找到最近敌人后显示 mask。

**优化/延伸**:
- 变量语义要清晰,「最近/最远」混用(`minDistance`/`maxDistance`)容易写出矛盾判断;
- 射线方向向量应做 `.normalized()` 归一化;
- 标记应每帧跟随最近敌人,且没有敌人时隐藏(`SetActive(false)`),否则状态不会复位。

---

<a id="bug-003"></a>

## BUG-003 复制多个敌人后报 NullReferenceException

**日期**:2026-11-07

**现象**:场景里多复制几个敌人后,报 `NullReferenceException`,堆栈定位在 `Enemy.Start () (Enemy.cs:13)`。

**排查过程**:
1. 报错在第 13 行 `player.enemyList.Add(this)`,而不是第 12 行 `player = GameManager.Instance.player`,说明 `GameManager.Instance` 不为 null,但 `player` 是 null;
2. 结合 `GameManager.cs`,发现 `Instance` 在 `Awake()` 赋值,`player` 字段在 `Player` 的 `Start()` 里才赋值;
3. 敌人和玩家各自 `Start()` 的执行顺序不确定,多个敌人时更容易出现「敌人 Start 先于玩家 Start」,此时读到的 `player` 还是 null。

**根因**:跨物体的脚本执行顺序不保证。`GameManager.player` 依赖 `Player.Start()` 赋值,而 `Enemy.Start()` / `PlayerAiM.Start()` 可能先执行,导致读到 null。

**解决方案**(三选一,按推荐度排序):
1. **Inspector 直接拖拽**(最省事):把玩家物体拖到 GameManager 的 `player` 字段,不依赖代码赋值;
2. **调整 Script Execution Order**:让 `GameManager` 和 `Player` 的执行顺序为负数(早于默认时间 0),保证先初始化;
3. **把赋值从 Start 挪到 Awake**:Unity 保证所有物体的 `Awake()` 先于所有 `Start()` 执行,更稳妥。

**优化/延伸** —— **懒加载单例**(项目变大后的推荐做法):

执行顺序不可靠,真正的工程化方案是「让代码不依赖执行顺序」:

```csharp
// GameManager 懒加载单例:谁先访问都能拿到实例
private static GameManager _instance;
public static GameManager Instance
{
    get
    {
        if (_instance == null)
            _instance = FindObjectOfType<GameManager>();
        return _instance;
    }
}
```

**通用经验总结**:
- **初始化(拿引用、注册自己)统一放 `Awake()`,而不是 `Start()`** —— 这是避免执行顺序问题最核心的习惯;
- 单例尽量用懒加载 + 空值保护,避免「A 先访问 B 未初始化」的时序 bug;
- 项目变大后,不要靠挨个调 Script Execution Order 去凑顺序,而是让代码健壮、不依赖顺序;
- 访问单例字段前加判空(`if (Instance != null && Instance.player != null)`),能显著减少踩坑。

---

<!-- 后续新增 bug 请按以下模板追加:

## BUG-XXX 标题

**日期**:

**现象**:

**排查过程**:

**根因**:

**解决方案**:

**优化/延伸**:

-->
