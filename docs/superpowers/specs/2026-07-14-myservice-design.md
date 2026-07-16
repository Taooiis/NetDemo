# MyService 微服务设计文档

## 1. 概述

- **项目名称**: MyService
- **项目类型**: 基于 EF Core Code First 的 DDD 微服务
- **核心功能**: 用户管理微服务
- **目标用户**: 内部业务系统

## 2. 技术栈

| 技术 | 版本 |
|------|------|
| .NET | 8.0 |
| C# | 12 |
| EF Core | 8.0 |
| MySQL | 8.0 |
| Swagger/OpenAPI | NSwag |

## 3. 架构

采用 **DDD（D三步域驱动设计）** 分层架构。

```
MyService.sln
├── src/
│   ├── MyService.Api/           # API 层： Controllers、Middleware、Swagger
│   ├── MyService.Domain/         # 领域层： Entities、Interfaces、Events
│   ├── MyService.Application/   # 应用层： Services、DTOs
│   ├── MyService.Infrastructure/ # 基础设施： EF Core、Repositories
│   └── MyService.Common/         # 通用工具
└── tests/
    ├── MyService.Domain.UnitTests/
    └── MyService.Application.UnitTests/
```

### 3.1 分层职责

| 层 | 职责 |
|----|------|
| Api | HTTP 请求处理、路由、 Swagger 文档、异常中间件 |
| Application | 用例编排、DTO 转换、应用服务接口 |
| Domain | 实体定义、仓储接口、领域事件、值对象 |
| Infrastructure | EF Core DbContext、仓储实现、数据库配置 |
| Common | 通用工具类、扩展方法 |

## 4. 核心实体

### 4.1 User 实体

| 字段 | 类型 | 约束 |
|------|------|------|
| Id | Guid | PK |
| Username | string | Max(50), Unique |
| Email | string | Max(100), Unique |
| PasswordHash | string | Max(256) |
| CreatedAt | DateTime | Required |
| UpdatedAt | DateTime | Required |

## 5. API 设计

### 5.1 用户管理端点

| 方法 | 路径 | 描述 |
|------|------|------|
| GET | /api/users | 获取用户列表 |
| GET | /api/users/{id} | 获取单个用户 |
| POST | /api/users | 创建用户 |
| PUT | /api/users/{id} | 更新用户 |
| DELETE | /api/users/{id} | 删除用户 |

### 5.2 请求/响应 DTOs

- `UserDto` - 用户数据传输对象
- `CreateUserRequest` - 创建用户请求
- `UpdateUserRequest` - 更新用户请求

## 6. 数据库

- **数据库类型**: MySQL 8.0
- **迁移策略**: EF Core Code First + Migrations
- **连接字符串**: 配置在 `appsettings.json`

## 7. Swagger 配置

- 使用 NSwag 实现 OpenAPI 3.0 文档
- 访问地址: `/swagger`
- 包含 XML 注释

## 8. 错误处理

- 全局异常中间件
- 统一错误响应格式
- 日志记录

## 9. 项目引用关系

```
Api → Application → Domain
Api → Infrastructure
Application → Domain
Infrastructure → Domain
```