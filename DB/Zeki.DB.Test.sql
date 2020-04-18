USE [Test]
GO
/****** Object:  Table [dbo].[sysDepartment]    Script Date: 2018/12/8 17:03:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[sysDepartment](
	[depId] [int] IDENTITY(1,1) NOT NULL,
	[depPid] [int] NOT NULL,
	[depName] [nvarchar](50) NOT NULL,
	[depRemark] [nvarchar](200) NULL,
	[depIsDel] [bit] NOT NULL,
	[depAddTime] [datetime] NOT NULL,
 CONSTRAINT [PK_Department] PRIMARY KEY CLUSTERED 
(
	[depId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[sysPermission]    Script Date: 2018/12/8 17:03:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[sysPermission](
	[pId] [int] IDENTITY(1,1) NOT NULL,
	[pParent] [int] NOT NULL,
	[pName] [nvarchar](50) NOT NULL,
	[pAreaName] [nvarchar](50) NULL,
	[pControllerName] [nvarchar](50) NULL,
	[pActionName] [nvarchar](50) NULL,
	[pFormMethod] [int] NOT NULL,
	[pFunction] [nvarchar](50) NULL,
	[pFunName] [nvarchar](50) NULL,
	[pPicName] [nvarchar](50) NULL,
	[pOrder] [int] NOT NULL,
	[pIsShow] [bit] NULL,
	[pRemark] [nvarchar](200) NULL,
	[pIsDel] [bit] NOT NULL,
	[pAddTime] [datetime] NOT NULL,
 CONSTRAINT [PK_Permission] PRIMARY KEY CLUSTERED 
(
	[pId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[sysRole_1]    Script Date: 2018/12/8 17:03:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[sysRole_1](
	[rId] [int] IDENTITY(1,1) NOT NULL,
	[rDepId] [int] NOT NULL,
	[rName] [nvarchar](50) NOT NULL,
	[rRemark] [nvarchar](200) NULL,
	[rIsDel] [bit] NOT NULL,
	[rAddTime] [datetime] NOT NULL,
 CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED 
(
	[rId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[sysUserInfo]    Script Date: 2018/12/8 17:03:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[sysUserInfo](
	[uId] [int] IDENTITY(1,1) NOT NULL,
	[uDepId] [int] NOT NULL,
	[uLoginName] [nvarchar](50) NOT NULL,
	[uPwd] [char](32) NOT NULL,
	[uGender] [bit] NOT NULL,
	[uEmail] [nvarchar](50) NULL,
	[uRemark] [nvarchar](200) NULL,
	[uIsDel] [bit] NOT NULL,
	[uAddTime] [datetime] NOT NULL,
 CONSTRAINT [PK_UserInfo] PRIMARY KEY CLUSTERED 
(
	[uId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
SET IDENTITY_INSERT [dbo].[sysDepartment] ON 

INSERT [dbo].[sysDepartment] ([depId], [depPid], [depName], [depRemark], [depIsDel], [depAddTime]) VALUES (1, 1, N'公司', N'就是公司了', 0, CAST(0x0000A34900E15167 AS DateTime))
INSERT [dbo].[sysDepartment] ([depId], [depPid], [depName], [depRemark], [depIsDel], [depAddTime]) VALUES (6, 1, N'财务部', N'财务部', 0, CAST(0x0000A34E00F1AA16 AS DateTime))
INSERT [dbo].[sysDepartment] ([depId], [depPid], [depName], [depRemark], [depIsDel], [depAddTime]) VALUES (9, 1, N'技术部', N'搞技术', 0, CAST(0x0000A41B0133AE47 AS DateTime))
SET IDENTITY_INSERT [dbo].[sysDepartment] OFF
SET IDENTITY_INSERT [dbo].[sysPermission] ON 

INSERT [dbo].[sysPermission] ([pId], [pParent], [pName], [pAreaName], [pControllerName], [pActionName], [pFormMethod], [pFunction], [pFunName], [pPicName], [pOrder], [pIsShow], [pRemark], [pIsDel], [pAddTime]) VALUES (1, 0, N'系统管理', NULL, NULL, NULL, 3, NULL, NULL, NULL, 10001, 1, N'管理后台首页', 0, CAST(0x0000A34900E3F8C2 AS DateTime))
INSERT [dbo].[sysPermission] ([pId], [pParent], [pName], [pAreaName], [pControllerName], [pActionName], [pFormMethod], [pFunction], [pFunName], [pPicName], [pOrder], [pIsShow], [pRemark], [pIsDel], [pAddTime]) VALUES (3, 1, N'角色管理', N'Admin', N'Role', N'Index', 3, NULL, NULL, NULL, 10300, 1, N'首页[get]和获取分页数据[post]', 0, CAST(0x0000A34900E4872A AS DateTime))
INSERT [dbo].[sysPermission] ([pId], [pParent], [pName], [pAreaName], [pControllerName], [pActionName], [pFormMethod], [pFunction], [pFunName], [pPicName], [pOrder], [pIsShow], [pRemark], [pIsDel], [pAddTime]) VALUES (4, 1, N'权限管理', N'Admin', N'Permission', N'Index', 3, NULL, NULL, NULL, 10400, 1, N'首页[get]和获取分页数据[post]', 0, CAST(0x0000A34900E4A83B AS DateTime))
INSERT [dbo].[sysPermission] ([pId], [pParent], [pName], [pAreaName], [pControllerName], [pActionName], [pFormMethod], [pFunction], [pFunName], [pPicName], [pOrder], [pIsShow], [pRemark], [pIsDel], [pAddTime]) VALUES (5, 1, N'部门管理', N'Admin', N'Department', N'Index', 3, NULL, NULL, NULL, 10500, 1, N'首页[get]和获取分页数据[post]', 0, CAST(0x0000A34900E4D313 AS DateTime))
INSERT [dbo].[sysPermission] ([pId], [pParent], [pName], [pAreaName], [pControllerName], [pActionName], [pFormMethod], [pFunction], [pFunName], [pPicName], [pOrder], [pIsShow], [pRemark], [pIsDel], [pAddTime]) VALUES (6, 1, N'用户管理', N'Admin', N'User', N'Index', 3, NULL, NULL, NULL, 10642, 1, N'首页[get]和获取分页数据[post]', 0, CAST(0x0000A34900E4EEE6 AS DateTime))
INSERT [dbo].[sysPermission] ([pId], [pParent], [pName], [pAreaName], [pControllerName], [pActionName], [pFormMethod], [pFunction], [pFunName], [pPicName], [pOrder], [pIsShow], [pRemark], [pIsDel], [pAddTime]) VALUES (7, 1, N'特权管理', N'Admin', N'VipPer', N'Index', 3, NULL, NULL, NULL, 10642, 1, N'首页[get]和获取分页数据[post]', 0, CAST(0x0000A34900E50B0B AS DateTime))
INSERT [dbo].[sysPermission] ([pId], [pParent], [pName], [pAreaName], [pControllerName], [pActionName], [pFormMethod], [pFunction], [pFunName], [pPicName], [pOrder], [pIsShow], [pRemark], [pIsDel], [pAddTime]) VALUES (8, 6, N'设置角色', N'Admin', N'User', N'SetRole', 3, N'setRole', N'设置角色', N'icon-tip', 10609, 0, N'首页[get]和获取分页数据[post]', 0, CAST(0x0000A34900E5B9AE AS DateTime))
INSERT [dbo].[sysPermission] ([pId], [pParent], [pName], [pAreaName], [pControllerName], [pActionName], [pFormMethod], [pFunction], [pFunName], [pPicName], [pOrder], [pIsShow], [pRemark], [pIsDel], [pAddTime]) VALUES (10, 3, N'设置角色权限', N'Admin', N'Role', N'SetRolePers', 3, N'setRolePers', N'设置角色权限', N'icon-tip', 10309, 0, N'查看角色权限Get请求【设置角色权限】', 0, CAST(0x0000A34900E6807D AS DateTime))
INSERT [dbo].[sysPermission] ([pId], [pParent], [pName], [pAreaName], [pControllerName], [pActionName], [pFormMethod], [pFunction], [pFunName], [pPicName], [pOrder], [pIsShow], [pRemark], [pIsDel], [pAddTime]) VALUES (13, 4, N'新增权限', N'Admin', N'Permission', N'Add', 3, N'add', N'新增', N'icon-add', 10402, 0, NULL, 0, CAST(0x0000A34900E861FA AS DateTime))
INSERT [dbo].[sysPermission] ([pId], [pParent], [pName], [pAreaName], [pControllerName], [pActionName], [pFormMethod], [pFunction], [pFunName], [pPicName], [pOrder], [pIsShow], [pRemark], [pIsDel], [pAddTime]) VALUES (15, 4, N'删除权限', N'Admin', N'Permission', N'Del', 2, N'del', N'删除', N'icon-remove', 10450, 0, NULL, 0, CAST(0x0000A34900E88E5C AS DateTime))
INSERT [dbo].[sysPermission] ([pId], [pParent], [pName], [pAreaName], [pControllerName], [pActionName], [pFormMethod], [pFunction], [pFunName], [pPicName], [pOrder], [pIsShow], [pRemark], [pIsDel], [pAddTime]) VALUES (17, 4, N'查看子权限', N'Admin', N'Permission', N'IndexSon', 3, N'lookSonPer', N'查看子权限', N'icon-search', 10642, 0, NULL, 0, CAST(0x0000A34900E8D629 AS DateTime))
INSERT [dbo].[sysPermission] ([pId], [pParent], [pName], [pAreaName], [pControllerName], [pActionName], [pFormMethod], [pFunction], [pFunName], [pPicName], [pOrder], [pIsShow], [pRemark], [pIsDel], [pAddTime]) VALUES (19, 3, N'删除角色', N'Admin', N'Role', N'Del', 2, N'del', N'删除', N'icon-remove', 10308, 0, NULL, 0, CAST(0x0000A34900E980B0 AS DateTime))
INSERT [dbo].[sysPermission] ([pId], [pParent], [pName], [pAreaName], [pControllerName], [pActionName], [pFormMethod], [pFunction], [pFunName], [pPicName], [pOrder], [pIsShow], [pRemark], [pIsDel], [pAddTime]) VALUES (20, 3, N'新增角色', N'Admin', N'Role', N'Add', 3, N'add', N'新增', N'icon-add', 10305, 0, NULL, 0, CAST(0x0000A34900E9A3CA AS DateTime))
INSERT [dbo].[sysPermission] ([pId], [pParent], [pName], [pAreaName], [pControllerName], [pActionName], [pFormMethod], [pFunction], [pFunName], [pPicName], [pOrder], [pIsShow], [pRemark], [pIsDel], [pAddTime]) VALUES (24, 3, N'修改角色', N'Admin', N'Role', N'Modify', 3, N'modify', N'修改', N'icon-edit', 10307, 0, NULL, 0, CAST(0x0000A34900EACC57 AS DateTime))
INSERT [dbo].[sysPermission] ([pId], [pParent], [pName], [pAreaName], [pControllerName], [pActionName], [pFormMethod], [pFunction], [pFunName], [pPicName], [pOrder], [pIsShow], [pRemark], [pIsDel], [pAddTime]) VALUES (29, 0, N'系统操作', NULL, NULL, NULL, 1, NULL, NULL, NULL, 12100, 1, N'系统操作', 0, CAST(0x0000A34900EB9DFD AS DateTime))
INSERT [dbo].[sysPermission] ([pId], [pParent], [pName], [pAreaName], [pControllerName], [pActionName], [pFormMethod], [pFunction], [pFunName], [pPicName], [pOrder], [pIsShow], [pRemark], [pIsDel], [pAddTime]) VALUES (31, 29, N'修改密码', N'Sys', N'Operation', N'ChangePwd', 1, NULL, NULL, NULL, 12101, 1, NULL, 0, CAST(0x0000A34900EBD492 AS DateTime))
INSERT [dbo].[sysPermission] ([pId], [pParent], [pName], [pAreaName], [pControllerName], [pActionName], [pFormMethod], [pFunction], [pFunName], [pPicName], [pOrder], [pIsShow], [pRemark], [pIsDel], [pAddTime]) VALUES (32, 29, N'修改用户名', N'Sys', N'Operation', N'ChangeuName', 3, NULL, NULL, NULL, 12102, 1, NULL, 0, CAST(0x0000A34900EDEEA8 AS DateTime))
INSERT [dbo].[sysPermission] ([pId], [pParent], [pName], [pAreaName], [pControllerName], [pActionName], [pFormMethod], [pFunction], [pFunName], [pPicName], [pOrder], [pIsShow], [pRemark], [pIsDel], [pAddTime]) VALUES (34, 5, N'新增部门', N'Admin', N'Department', N'Add', 3, N'add', N'新增', N'icon-add', 10501, 0, NULL, 0, CAST(0x0000A34900F68EEB AS DateTime))
INSERT [dbo].[sysPermission] ([pId], [pParent], [pName], [pAreaName], [pControllerName], [pActionName], [pFormMethod], [pFunction], [pFunName], [pPicName], [pOrder], [pIsShow], [pRemark], [pIsDel], [pAddTime]) VALUES (35, 5, N'修改部门', N'Admin', N'Department', N'Modify', 3, N'modify', N'修改', N'icon-edit', 10502, 0, NULL, 0, CAST(0x0000A34900F6AF74 AS DateTime))
INSERT [dbo].[sysPermission] ([pId], [pParent], [pName], [pAreaName], [pControllerName], [pActionName], [pFormMethod], [pFunction], [pFunName], [pPicName], [pOrder], [pIsShow], [pRemark], [pIsDel], [pAddTime]) VALUES (36, 5, N'删除部门', N'Admin', N'Department', N'Del', 2, N'del', N'删除', N'icon-remove', 10503, 0, NULL, 0, CAST(0x0000A34900F6C563 AS DateTime))
INSERT [dbo].[sysPermission] ([pId], [pParent], [pName], [pAreaName], [pControllerName], [pActionName], [pFormMethod], [pFunction], [pFunName], [pPicName], [pOrder], [pIsShow], [pRemark], [pIsDel], [pAddTime]) VALUES (37, 7, N'新增特权', N'Admin', N'VipPer', N'Add', 3, N'add', N'新增', N'icon-add', 10704, 0, NULL, 0, CAST(0x0000A34900F6E584 AS DateTime))
INSERT [dbo].[sysPermission] ([pId], [pParent], [pName], [pAreaName], [pControllerName], [pActionName], [pFormMethod], [pFunction], [pFunName], [pPicName], [pOrder], [pIsShow], [pRemark], [pIsDel], [pAddTime]) VALUES (38, 6, N'新增用户', N'Admin', N'User', N'Add', 3, N'add', N'新增', N'icon-add', 10602, 0, NULL, 0, CAST(0x0000A34900F77684 AS DateTime))
INSERT [dbo].[sysPermission] ([pId], [pParent], [pName], [pAreaName], [pControllerName], [pActionName], [pFormMethod], [pFunction], [pFunName], [pPicName], [pOrder], [pIsShow], [pRemark], [pIsDel], [pAddTime]) VALUES (39, 6, N'修改用户', N'Admin', N'User', N'Modify', 3, N'modify', N'修改', N'icon-edit', 10603, 0, NULL, 0, CAST(0x0000A34900F78DCA AS DateTime))
INSERT [dbo].[sysPermission] ([pId], [pParent], [pName], [pAreaName], [pControllerName], [pActionName], [pFormMethod], [pFunction], [pFunName], [pPicName], [pOrder], [pIsShow], [pRemark], [pIsDel], [pAddTime]) VALUES (40, 6, N'删除用户', N'Admin', N'User', N'Del', 2, N'del', N'删除', N'icon-remove', 10604, 0, NULL, 0, CAST(0x0000A34900F79EED AS DateTime))
INSERT [dbo].[sysPermission] ([pId], [pParent], [pName], [pAreaName], [pControllerName], [pActionName], [pFormMethod], [pFunction], [pFunName], [pPicName], [pOrder], [pIsShow], [pRemark], [pIsDel], [pAddTime]) VALUES (99, 4, N'修改权限', N'Admin', N'Permission', N'Modify', 3, N'modify', N'修改', N'icon-edit', 10408, 0, NULL, 0, CAST(0x0000A34D014115ED AS DateTime))
INSERT [dbo].[sysPermission] ([pId], [pParent], [pName], [pAreaName], [pControllerName], [pActionName], [pFormMethod], [pFunction], [pFunName], [pPicName], [pOrder], [pIsShow], [pRemark], [pIsDel], [pAddTime]) VALUES (108, 4, N'新增子权限', N'Admin', N'Permission', N'AddSon', 3, N'addSon', N'新增', N'icon-add', 10490, 0, NULL, 0, CAST(0x0000A34D017C4B0D AS DateTime))
INSERT [dbo].[sysPermission] ([pId], [pParent], [pName], [pAreaName], [pControllerName], [pActionName], [pFormMethod], [pFunction], [pFunName], [pPicName], [pOrder], [pIsShow], [pRemark], [pIsDel], [pAddTime]) VALUES (110, 4, N'修改子权限', N'Admin', N'Permission', N'ModifySon', 3, N'modifySon', N'修改', N'icon-edit', 10591, 0, NULL, 0, CAST(0x0000A34D017C758B AS DateTime))
INSERT [dbo].[sysPermission] ([pId], [pParent], [pName], [pAreaName], [pControllerName], [pActionName], [pFormMethod], [pFunction], [pFunName], [pPicName], [pOrder], [pIsShow], [pRemark], [pIsDel], [pAddTime]) VALUES (111, 4, N'删除子权限', N'Admin', N'Permission', N'DelSon', 2, N'delSon', N'删除', N'icon-remove', 10642, 0, NULL, 0, CAST(0x0000A34D017C92C1 AS DateTime))
INSERT [dbo].[sysPermission] ([pId], [pParent], [pName], [pAreaName], [pControllerName], [pActionName], [pFormMethod], [pFunction], [pFunName], [pPicName], [pOrder], [pIsShow], [pRemark], [pIsDel], [pAddTime]) VALUES (121, 29, N'退出系统', N'Sys', N'Operation', N'LoginOut', 2, NULL, NULL, NULL, 12104, 1, NULL, 0, CAST(0x0000A34E013C61B8 AS DateTime))
INSERT [dbo].[sysPermission] ([pId], [pParent], [pName], [pAreaName], [pControllerName], [pActionName], [pFormMethod], [pFunction], [pFunName], [pPicName], [pOrder], [pIsShow], [pRemark], [pIsDel], [pAddTime]) VALUES (122, 7, N'修改特权', N'Admin', N'VipPer', N'Modify', 3, N'modify', N'修改', N'icon-edit', 10705, 0, NULL, 0, CAST(0x0000A34F0102F7DC AS DateTime))
INSERT [dbo].[sysPermission] ([pId], [pParent], [pName], [pAreaName], [pControllerName], [pActionName], [pFormMethod], [pFunction], [pFunName], [pPicName], [pOrder], [pIsShow], [pRemark], [pIsDel], [pAddTime]) VALUES (123, 7, N'删除特权', N'Admin', N'VipPer', N'Del', 2, N'del', N'删除', N'icon-remove', 10706, 0, NULL, 0, CAST(0x0000A34F01030F7D AS DateTime))
INSERT [dbo].[sysPermission] ([pId], [pParent], [pName], [pAreaName], [pControllerName], [pActionName], [pFormMethod], [pFunction], [pFunName], [pPicName], [pOrder], [pIsShow], [pRemark], [pIsDel], [pAddTime]) VALUES (124, 0, N'工作流管理', NULL, NULL, NULL, 3, NULL, NULL, NULL, 11000, 1, NULL, 0, CAST(0x0000A3AE01544AF9 AS DateTime))
INSERT [dbo].[sysPermission] ([pId], [pParent], [pName], [pAreaName], [pControllerName], [pActionName], [pFormMethod], [pFunction], [pFunName], [pPicName], [pOrder], [pIsShow], [pRemark], [pIsDel], [pAddTime]) VALUES (125, 124, N'工作流设定', N'WorkFlow', N'Manage', N'Index', 3, NULL, NULL, NULL, 11100, 1, N'工作流节点/分支设定', 0, CAST(0x0000A3AE0154A89F AS DateTime))
INSERT [dbo].[sysPermission] ([pId], [pParent], [pName], [pAreaName], [pControllerName], [pActionName], [pFormMethod], [pFunction], [pFunName], [pPicName], [pOrder], [pIsShow], [pRemark], [pIsDel], [pAddTime]) VALUES (127, 124, N'我的申请单', N'WorkFlow', N'RequestForm', N'Index', 3, NULL, NULL, NULL, 11200, 1, NULL, 0, CAST(0x0000A3AE01551498 AS DateTime))
INSERT [dbo].[sysPermission] ([pId], [pParent], [pName], [pAreaName], [pControllerName], [pActionName], [pFormMethod], [pFunction], [pFunName], [pPicName], [pOrder], [pIsShow], [pRemark], [pIsDel], [pAddTime]) VALUES (129, 124, N'我的审核单', N'WorkFlow', N'Process', N'Index', 3, NULL, NULL, NULL, 11300, 1, NULL, 0, CAST(0x0000A3AE01559AAC AS DateTime))
INSERT [dbo].[sysPermission] ([pId], [pParent], [pName], [pAreaName], [pControllerName], [pActionName], [pFormMethod], [pFunction], [pFunName], [pPicName], [pOrder], [pIsShow], [pRemark], [pIsDel], [pAddTime]) VALUES (130, 125, N'新增工作流', N'WorkFlow', N'Manage', N'Add', 3, N'add', N'新增', N'icon-add', 11101, 0, NULL, 0, CAST(0x0000A3AE01571A3F AS DateTime))
INSERT [dbo].[sysPermission] ([pId], [pParent], [pName], [pAreaName], [pControllerName], [pActionName], [pFormMethod], [pFunction], [pFunName], [pPicName], [pOrder], [pIsShow], [pRemark], [pIsDel], [pAddTime]) VALUES (131, 125, N'修改工作流', N'WorkFlow', N'Manage', N'Modify', 3, N'modify', N'修改', N'icon-edit', 11102, 0, NULL, 0, CAST(0x0000A3AE01574B29 AS DateTime))
INSERT [dbo].[sysPermission] ([pId], [pParent], [pName], [pAreaName], [pControllerName], [pActionName], [pFormMethod], [pFunction], [pFunName], [pPicName], [pOrder], [pIsShow], [pRemark], [pIsDel], [pAddTime]) VALUES (132, 125, N'删除工作流', N'WorkFlow', N'Manage', N'Del', 2, N'del', N'删除', N'icon-remove', 11103, 0, NULL, 0, CAST(0x0000A3AE0157762F AS DateTime))
INSERT [dbo].[sysPermission] ([pId], [pParent], [pName], [pAreaName], [pControllerName], [pActionName], [pFormMethod], [pFunction], [pFunName], [pPicName], [pOrder], [pIsShow], [pRemark], [pIsDel], [pAddTime]) VALUES (134, 125, N'设置流程节点', N'WorkFlow', N'Manage', N'SetNodes', 3, N'setNodes', N'设置流程节点', N'icon-tip', 11104, 0, NULL, 0, CAST(0x0000A3AE0157BC9C AS DateTime))
INSERT [dbo].[sysPermission] ([pId], [pParent], [pName], [pAreaName], [pControllerName], [pActionName], [pFormMethod], [pFunction], [pFunName], [pPicName], [pOrder], [pIsShow], [pRemark], [pIsDel], [pAddTime]) VALUES (136, 127, N'新增申请单', N'WorkFlow', N'RequestForm', N'Add', 3, N'add', N'新增', N'icon-add', 11201, 0, NULL, 0, CAST(0x0000A3AE01597983 AS DateTime))
INSERT [dbo].[sysPermission] ([pId], [pParent], [pName], [pAreaName], [pControllerName], [pActionName], [pFormMethod], [pFunction], [pFunName], [pPicName], [pOrder], [pIsShow], [pRemark], [pIsDel], [pAddTime]) VALUES (138, 127, N'删除申请单', N'WorkFlow', N'RequestForm', N'Del', 2, N'del ', N'删除', N'icon-remove', 11203, 0, NULL, 0, CAST(0x0000A3AE0159CD26 AS DateTime))
INSERT [dbo].[sysPermission] ([pId], [pParent], [pName], [pAreaName], [pControllerName], [pActionName], [pFormMethod], [pFunction], [pFunName], [pPicName], [pOrder], [pIsShow], [pRemark], [pIsDel], [pAddTime]) VALUES (139, 127, N'查看审核明细', N'WorkFlow', N'RequestForm', N'GetDetail', 1, N'getDetail', N'查看审核明细', N'icon-search', 11204, 0, NULL, 0, CAST(0x0000A3AE015ABA8F AS DateTime))
INSERT [dbo].[sysPermission] ([pId], [pParent], [pName], [pAreaName], [pControllerName], [pActionName], [pFormMethod], [pFunction], [pFunName], [pPicName], [pOrder], [pIsShow], [pRemark], [pIsDel], [pAddTime]) VALUES (140, 129, N'审核', N'WorkFlow', N'Process', N'CheckForm', 3, N'checkForm', N'审核', N'icon-ok', 11301, 0, NULL, 0, CAST(0x0000A3AE015CB720 AS DateTime))
INSERT [dbo].[sysPermission] ([pId], [pParent], [pName], [pAreaName], [pControllerName], [pActionName], [pFormMethod], [pFunction], [pFunName], [pPicName], [pOrder], [pIsShow], [pRemark], [pIsDel], [pAddTime]) VALUES (141, 1, N'字典管理', N'Admin', N'KeyValue', N'Index', 3, NULL, NULL, NULL, 10700, 1, NULL, 0, CAST(0x0000A3AE0177D162 AS DateTime))
INSERT [dbo].[sysPermission] ([pId], [pParent], [pName], [pAreaName], [pControllerName], [pActionName], [pFormMethod], [pFunction], [pFunName], [pPicName], [pOrder], [pIsShow], [pRemark], [pIsDel], [pAddTime]) VALUES (142, 141, N'新增字典', N'Admin', N'KeyValue', N'Add', 3, N'add', N'新增', N'icon-add', 10701, 0, NULL, 0, CAST(0x0000A3AE01780A67 AS DateTime))
INSERT [dbo].[sysPermission] ([pId], [pParent], [pName], [pAreaName], [pControllerName], [pActionName], [pFormMethod], [pFunction], [pFunName], [pPicName], [pOrder], [pIsShow], [pRemark], [pIsDel], [pAddTime]) VALUES (143, 141, N'修改字典', N'Admin', N'KeyValue', N'Modify', 3, N'modify', N'修改', N'icon-edit', 10702, 0, NULL, 0, CAST(0x0000A3AE017833C0 AS DateTime))
INSERT [dbo].[sysPermission] ([pId], [pParent], [pName], [pAreaName], [pControllerName], [pActionName], [pFormMethod], [pFunction], [pFunName], [pPicName], [pOrder], [pIsShow], [pRemark], [pIsDel], [pAddTime]) VALUES (144, 141, N'删除字典', N'Admin', N'KeyValue', N'Del', 3, N'del', N'删除', N'icon-remove', 10703, 0, NULL, 0, CAST(0x0000A3AE017F9FAF AS DateTime))
INSERT [dbo].[sysPermission] ([pId], [pParent], [pName], [pAreaName], [pControllerName], [pActionName], [pFormMethod], [pFunction], [pFunName], [pPicName], [pOrder], [pIsShow], [pRemark], [pIsDel], [pAddTime]) VALUES (145, 127, N'查看审批流程', N'WorkFlow', N'RequestForm', N'ViewFlow', 1, N'viewFlow', N'查看审批流程', N'icon-search', 11205, 0, NULL, 0, CAST(0x0000A3B2015A6F9D AS DateTime))
SET IDENTITY_INSERT [dbo].[sysPermission] OFF
SET IDENTITY_INSERT [dbo].[sysRole_1] ON 

INSERT [dbo].[sysRole_1] ([rId], [rDepId], [rName], [rRemark], [rIsDel], [rAddTime]) VALUES (1, 9, N'项目经理', N'技术部项目经理', 0, CAST(0x0000A34900E24786 AS DateTime))
INSERT [dbo].[sysRole_1] ([rId], [rDepId], [rName], [rRemark], [rIsDel], [rAddTime]) VALUES (34, 6, N'财务总监', N'财务总监', 0, CAST(0x0000A352009EC8DF AS DateTime))
INSERT [dbo].[sysRole_1] ([rId], [rDepId], [rName], [rRemark], [rIsDel], [rAddTime]) VALUES (35, 9, N'技术总监', N'总监', 0, CAST(0x0000A352009ED341 AS DateTime))
INSERT [dbo].[sysRole_1] ([rId], [rDepId], [rName], [rRemark], [rIsDel], [rAddTime]) VALUES (40, 9, N'技术主管', N'主管', 0, CAST(0x0000A3AD011B68A1 AS DateTime))
INSERT [dbo].[sysRole_1] ([rId], [rDepId], [rName], [rRemark], [rIsDel], [rAddTime]) VALUES (44, 6, N'财务主管', N'财务主管', 0, CAST(0x0000A3AF016B0855 AS DateTime))
INSERT [dbo].[sysRole_1] ([rId], [rDepId], [rName], [rRemark], [rIsDel], [rAddTime]) VALUES (46, 1, N'职员', NULL, 0, CAST(0x0000A3AF016B533A AS DateTime))
INSERT [dbo].[sysRole_1] ([rId], [rDepId], [rName], [rRemark], [rIsDel], [rAddTime]) VALUES (47, 9, N'项目组长', NULL, 0, CAST(0x0000A3AF016B6A48 AS DateTime))
INSERT [dbo].[sysRole_1] ([rId], [rDepId], [rName], [rRemark], [rIsDel], [rAddTime]) VALUES (49, 9, N'技术经理', NULL, 0, CAST(0x0000A3AF016B9CD1 AS DateTime))
INSERT [dbo].[sysRole_1] ([rId], [rDepId], [rName], [rRemark], [rIsDel], [rAddTime]) VALUES (51, 6, N'财务经理', N'财务', 0, CAST(0x0000A3B301384A24 AS DateTime))
INSERT [dbo].[sysRole_1] ([rId], [rDepId], [rName], [rRemark], [rIsDel], [rAddTime]) VALUES (52, 1, N'其他', N'用于工作流审批结束角色', 0, CAST(0x0000A3B30164C059 AS DateTime))
INSERT [dbo].[sysRole_1] ([rId], [rDepId], [rName], [rRemark], [rIsDel], [rAddTime]) VALUES (57, 1, N'管理员', N'最高权限', 0, CAST(0x0000A41B01349487 AS DateTime))
SET IDENTITY_INSERT [dbo].[sysRole_1] OFF
SET IDENTITY_INSERT [dbo].[sysUserInfo] ON 

INSERT [dbo].[sysUserInfo] ([uId], [uDepId], [uLoginName], [uPwd], [uGender], [uEmail], [uRemark], [uIsDel], [uAddTime]) VALUES (2, 1, N'zzq', N'96E79218965EB72C92A549DD5A330112', 1, N'1269021626@qq.com', N'张良', 0, CAST(0x0000A34900E1C796 AS DateTime))
INSERT [dbo].[sysUserInfo] ([uId], [uDepId], [uLoginName], [uPwd], [uGender], [uEmail], [uRemark], [uIsDel], [uAddTime]) VALUES (3, 6, N'cwjl', N'96E79218965EB72C92A549DD5A330112', 0, N'902@qq.com', N'人事', 0, CAST(0x0000A34F00E204AE AS DateTime))
INSERT [dbo].[sysUserInfo] ([uId], [uDepId], [uLoginName], [uPwd], [uGender], [uEmail], [uRemark], [uIsDel], [uAddTime]) VALUES (4, 1, N'zhiyuan', N'96E79218965EB72C92A549DD5A330112', 1, N'lijaing@qq.com', N'李江', 0, CAST(0x0000A34900E2B256 AS DateTime))
INSERT [dbo].[sysUserInfo] ([uId], [uDepId], [uLoginName], [uPwd], [uGender], [uEmail], [uRemark], [uIsDel], [uAddTime]) VALUES (71, 9, N'jszj', N'96E79218965EB72C92A549DD5A330112', 0, N'12@qq.com', N'dfsdfs12', 0, CAST(0x0000A34E01184BE4 AS DateTime))
INSERT [dbo].[sysUserInfo] ([uId], [uDepId], [uLoginName], [uPwd], [uGender], [uEmail], [uRemark], [uIsDel], [uAddTime]) VALUES (78, 6, N'cwzg', N'96E79218965EB72C92A549DD5A330112', 1, NULL, NULL, 0, CAST(0x0000A35200B748BE AS DateTime))
INSERT [dbo].[sysUserInfo] ([uId], [uDepId], [uLoginName], [uPwd], [uGender], [uEmail], [uRemark], [uIsDel], [uAddTime]) VALUES (79, 9, N'jsjl', N'96E79218965EB72C92A549DD5A330112', 1, NULL, NULL, 0, CAST(0x0000A35200B75944 AS DateTime))
INSERT [dbo].[sysUserInfo] ([uId], [uDepId], [uLoginName], [uPwd], [uGender], [uEmail], [uRemark], [uIsDel], [uAddTime]) VALUES (81, 6, N'cwzj', N'96E79218965EB72C92A549DD5A330112', 1, NULL, NULL, 0, CAST(0x0000A41B0104DEBA AS DateTime))
INSERT [dbo].[sysUserInfo] ([uId], [uDepId], [uLoginName], [uPwd], [uGender], [uEmail], [uRemark], [uIsDel], [uAddTime]) VALUES (82, 9, N'xmzz', N'96E79218965EB72C92A549DD5A330112', 1, NULL, NULL, 0, CAST(0x0000A41B0106C89C AS DateTime))
INSERT [dbo].[sysUserInfo] ([uId], [uDepId], [uLoginName], [uPwd], [uGender], [uEmail], [uRemark], [uIsDel], [uAddTime]) VALUES (83, 9, N'xmjl', N'96E79218965EB72C92A549DD5A330112', 1, NULL, NULL, 0, CAST(0x0000A41B0106DF09 AS DateTime))
INSERT [dbo].[sysUserInfo] ([uId], [uDepId], [uLoginName], [uPwd], [uGender], [uEmail], [uRemark], [uIsDel], [uAddTime]) VALUES (88, 2, N'zzq', N'jsdjfiu                         ', 0, N'12@qq.com', N'ceshi1', 0, CAST(0x0000A9B000ECD648 AS DateTime))
INSERT [dbo].[sysUserInfo] ([uId], [uDepId], [uLoginName], [uPwd], [uGender], [uEmail], [uRemark], [uIsDel], [uAddTime]) VALUES (89, 2, N'lisi', N'jsdjfiu                         ', 1, N'@@@@qq.com', N'ceshi1', 0, CAST(0x0000A9B000ECD648 AS DateTime))
INSERT [dbo].[sysUserInfo] ([uId], [uDepId], [uLoginName], [uPwd], [uGender], [uEmail], [uRemark], [uIsDel], [uAddTime]) VALUES (90, 2, N'zzq', N'jsdjfiu                         ', 0, N'12@qq.com', N'ceshi1', 0, CAST(0x0000A9B000EEAE9D AS DateTime))
INSERT [dbo].[sysUserInfo] ([uId], [uDepId], [uLoginName], [uPwd], [uGender], [uEmail], [uRemark], [uIsDel], [uAddTime]) VALUES (92, 2, N'zz1q', N'jsdjf1iu                        ', 0, N'1222@qq.com', N'ces2i1', 0, CAST(0x0000A9B0010085A8 AS DateTime))
INSERT [dbo].[sysUserInfo] ([uId], [uDepId], [uLoginName], [uPwd], [uGender], [uEmail], [uRemark], [uIsDel], [uAddTime]) VALUES (95, 2, N'zz1q', N'jsdjf1iu                        ', 0, N'1222@qq.com', N'ces2i1', 0, CAST(0x0000A9B0010664AB AS DateTime))
INSERT [dbo].[sysUserInfo] ([uId], [uDepId], [uLoginName], [uPwd], [uGender], [uEmail], [uRemark], [uIsDel], [uAddTime]) VALUES (96, 2, N'zz1q', N'jsdjf1iu                        ', 0, N'1222@qq.com', N'ces2i1', 0, CAST(0x0000A9B0010664AB AS DateTime))
SET IDENTITY_INSERT [dbo].[sysUserInfo] OFF
ALTER TABLE [dbo].[sysDepartment] ADD  CONSTRAINT [DF_Department_depPid]  DEFAULT ((-1)) FOR [depPid]
GO
ALTER TABLE [dbo].[sysDepartment] ADD  CONSTRAINT [DF_Department_depIsDel]  DEFAULT ((0)) FOR [depIsDel]
GO
ALTER TABLE [dbo].[sysDepartment] ADD  CONSTRAINT [DF_Department_depAddTime]  DEFAULT (getdate()) FOR [depAddTime]
GO
ALTER TABLE [dbo].[sysPermission] ADD  CONSTRAINT [DF_Permission_pOrder]  DEFAULT ((10001)) FOR [pOrder]
GO
ALTER TABLE [dbo].[sysPermission] ADD  CONSTRAINT [DF_sysPermission_pIsShow]  DEFAULT ((0)) FOR [pIsShow]
GO
ALTER TABLE [dbo].[sysPermission] ADD  CONSTRAINT [DF_Permission_pIsDel]  DEFAULT ((0)) FOR [pIsDel]
GO
ALTER TABLE [dbo].[sysPermission] ADD  CONSTRAINT [DF_Permission_pAddTime]  DEFAULT (getdate()) FOR [pAddTime]
GO
ALTER TABLE [dbo].[sysRole_1] ADD  CONSTRAINT [DF_Role_rIsDel]  DEFAULT ((0)) FOR [rIsDel]
GO
ALTER TABLE [dbo].[sysRole_1] ADD  CONSTRAINT [DF_Role_rAddTime]  DEFAULT (getdate()) FOR [rAddTime]
GO
ALTER TABLE [dbo].[sysUserInfo] ADD  CONSTRAINT [DF_UserInfo_uIsDel]  DEFAULT ((0)) FOR [uIsDel]
GO
ALTER TABLE [dbo].[sysUserInfo] ADD  CONSTRAINT [DF_UserInfo_uAddTime]  DEFAULT (getdate()) FOR [uAddTime]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'父Id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'sysDepartment', @level2type=N'COLUMN',@level2name=N'depPid'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'部门名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'sysDepartment', @level2type=N'COLUMN',@level2name=N'depName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'权限Id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'sysPermission', @level2type=N'COLUMN',@level2name=N'pId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'父Id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'sysPermission', @level2type=N'COLUMN',@level2name=N'pParent'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'权限名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'sysPermission', @level2type=N'COLUMN',@level2name=N'pName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'区域名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'sysPermission', @level2type=N'COLUMN',@level2name=N'pAreaName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'控制器名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'sysPermission', @level2type=N'COLUMN',@level2name=N'pControllerName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'方法名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'sysPermission', @level2type=N'COLUMN',@level2name=N'pActionName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'请求方式【1—get,2—Post,3—both】' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'sysPermission', @level2type=N'COLUMN',@level2name=N'pFormMethod'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'JS方法名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'sysPermission', @level2type=N'COLUMN',@level2name=N'pFunction'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'在工具条上显示的方法名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'sysPermission', @level2type=N'COLUMN',@level2name=N'pFunName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'图标名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'sysPermission', @level2type=N'COLUMN',@level2name=N'pPicName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'序号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'sysPermission', @level2type=N'COLUMN',@level2name=N'pOrder'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否显示' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'sysPermission', @level2type=N'COLUMN',@level2name=N'pIsShow'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'角色民' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'sysRole_1', @level2type=N'COLUMN',@level2name=N'rName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'部门Id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'sysUserInfo', @level2type=N'COLUMN',@level2name=N'uDepId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'密码' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'sysUserInfo', @level2type=N'COLUMN',@level2name=N'uPwd'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'性别【true为男,false为女】' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'sysUserInfo', @level2type=N'COLUMN',@level2name=N'uGender'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'邮箱' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'sysUserInfo', @level2type=N'COLUMN',@level2name=N'uEmail'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'备注' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'sysUserInfo', @level2type=N'COLUMN',@level2name=N'uRemark'
GO
