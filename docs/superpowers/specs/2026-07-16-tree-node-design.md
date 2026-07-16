# TreeNode 通用树结构设计文档

## 概述

实现一个通用六层树形结构，单表（邻接表 + 内存构建），支持 CRUD 和节点移动，附带基本信息和排序功能。

## 技术栈

- .NET 10 + EF Core 10 + MySQL
- DDD 分层架构（Api → Application → Domain → Infrastructure）

## 实体设计

### TreeNode

| 字段 | 类型 | 说明 |
|---|---|---|
| Id | Guid | 主键（继承 BaseEntity） |
| Name | string | 节点名称 |
| Code | string | 业务编码 |
| Description | string? | 描述 |
| Status | bool | 状态（true=启用, false=禁用） |
| ModelId | Guid? | 关联业务表 ID |
| ParentId | Guid? | 父节点 ID（null=根节点, Level 0） |
| Level | int | 层级（0~5） |
| SortOrder | int | 同级排序 |
| CreatedAt | DateTime | 创建时间（继承 BaseEntity） |
| UpdatedAt | DateTime | 更新时间（继承 BaseEntity） |

### 树规则

- Level 0 为根节点（ParentId == null），最大 Level 5
- 同级节点按 SortOrder 升序排列
- 删除节点时递归删除所有子节点

## API 设计

| 方法 | 路由 | 说明 |
|---|---|---|
| GET | /api/TreeNodes/GetTree | 返回整棵树（内存组装层级） |
| GET | /api/TreeNodes/GetById/{id} | 查单个节点 |
| GET | /api/TreeNodes/GetChildren/{parentId} | 查直接子节点 |
| POST | /api/TreeNodes/Create | 创建节点（自动计算 Level + SortOrder） |
| PUT | /api/TreeNodes/Update/{id} | 更新节点基本信息 |
| PUT | /api/TreeNodes/Move/{id} | 移动节点（换父节点/重排序） |
| DELETE | /api/TreeNodes/Delete/{id} | 删除节点及所有子节点 |

### DTO

**CreateTreeNodeRequest:** Name, Code, Description?, Status, ModelId?, ParentId?, SortOrder?（可选，默认排最后）

**UpdateTreeNodeRequest:** Name, Code, Description?, Status, ModelId?

**MoveTreeNodeRequest:** ParentId?, SortOrder

**TreeNodeDto:** Id, Name, Code, Description, Status, ModelId, ParentId, Level, SortOrder, Children (嵌套)

## 项目文件清单

1. `src/MyService.Domain/Entities/TreeNode.cs` — 实体
2. `src/MyService.Domain/Interfaces/ITreeNodeRepository.cs` — 仓储接口
3. `src/MyService.Application/DTOs/TreeNodeDtos.cs` — DTO
4. `src/MyService.Application/Interfaces/ITreeNodeService.cs` — 服务接口
5. `src/MyService.Application/Services/TreeNodeService.cs` — 服务实现
6. `src/MyService.Infrastructure/Data/Configurations/TreeNodeConfiguration.cs` — EF 配置
7. `src/MyService.Infrastructure/Repositories/TreeNodeRepository.cs` — 仓储实现
8. `src/MyService.Infrastructure/Extensions/ServiceCollectionExtensions.cs` — 更新 DI 注册
9. `src/MyService.Api/Controllers/TreeNodesController.cs` — 控制器
