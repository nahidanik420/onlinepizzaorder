create table tbl_user(
U_id int identity primary key,
u_name nvarchar(50) not null,
u_contact nvarchar(50) not null unique,
u_password nvarchar(50) not null,
)


create table tbl_product(
pro_id int identity primary key,
pro_name nvarchar(50) not null unique,
pro_price float,
pro_desc nvarchar(500) not null,
pro_image nvarchar(max),
)

create table tbl_order(
o_id int identity primary key,
o_fk_pro int foreign key references tbl_product(pro_id),
o_fk_invoice int foreign key references tbl_invoice(in_id),
o_date datetime,
o_qty int,
o_bill float,
o_unitprice int,
)

create table tbl_invoice(
in_id int identity primary key,
in_fk_user int foreign key references tbl_user(U_id),
in_date datetime,
in_totalbill float,
)
drop table tbl_user
