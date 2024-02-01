import xlrd
import csv
import codecs
import os
import pandas as pd
from openpyxl import Workbook
import json

# xlrd实现
def xlsx_to_csv(xlsx_dir_path, csv_dir_path):
    xlsx_suffix = '.xlsx'
    csv_suffix = '.csv'

    dir_list = os.listdir(xlsx_dir_path)  # dir_list中记录了所有的文件名 
    for xlsx_name in dir_list:
        if xlsx_suffix not in xlsx_name:  # 跳过不是.xlsx后缀的文件
            continue

        xlsx_path = os.path.join(xlsx_dir_path, xlsx_name) # 计算.xlsx路径
        csv_name = xlsx_name.replace(xlsx_suffix, csv_suffix)
        csv_path = os.path.join(csv_dir_path, csv_name)    # 计算csv路径

        workbook = xlrd.open_workbook(xlsx_path)
        table = workbook.sheet_by_index(0)             # 将.xlsx文件读取到表中
        with codecs.open(csv_path, 'w', encoding='utf-8') as f:
            write = csv.writer(f)
            for row_num in range(table.nrows):
                row_value = table.row_values(row_num)  # 以csv的格式读取.xlsx一行的值
                write.writerow(row_value)              # 写入.csv文件

# pandas实现
def xlsx_to_csv_pd(xlsx_dir_path, csv_dir_path):
    xlsx_suffix = '.xlsx'
    csv_suffix = '.csv'

    dir_list = os.listdir(xlsx_dir_path)
    for xlsx_name in dir_list:
        if xlsx_suffix not in xlsx_name:
            continue

        xlsx_path = os.path.join(xlsx_dir_path,xlsx_name)      # 计算.xlsx路径
        csv_name = xlsx_name.replace(xlsx_suffix, csv_suffix)
        csv_path = os.path.join(csv_dir_path, csv_name)        # 计算.csv路径

        data_xls = pd.read_excel(xlsx_path, index_col=0)
        data_xls.to_csv(csv_path, encoding='utf-8')

# openpyxl实现(由于xlwt库只支持.xls后缀，不支持.xlsx后缀)
def csv_to_xlsx(csv_dir_path, xlsx_dir_path):
    xlsx_suffix = '.xlsx'
    csv_suffix = '.csv'

    dir_list = os.listdir(csv_dir_path)  # dir_list中记录了所有的文件名 
    for csv_name in dir_list:
        if csv_suffix not in csv_name:  # 跳过不是.xlsx后缀的文件
            continue

        csv_path = os.path.join(csv_dir_path, csv_name)    # 计算.csv路径
        xlsx_name = csv_name.replace(csv_suffix, xlsx_suffix)
        xlsx_path = os.path.join(xlsx_dir_path, xlsx_name) # 计算.xlsx路径

        f = open(csv_path, 'r', encoding='utf-8')
        workbook = Workbook()
        worksheet = workbook.active
        workbook.title = 'sheet'

        for line in f:
            row = line.split(',')
            worksheet.append(row)
        
        f.close()
        workbook.save(xlsx_path)

# pandas实现（会自动生成第一列--不好用）
def csv_to_xlsx_pd(csv_dir_path, xlsx_dir_path):
    xlsx_suffix = '.xlsx'
    csv_suffix = '.csv'

    dir_list = os.listdir(csv_dir_path)  # dir_list中记录了所有的文件名 
    for csv_name in dir_list:
        if csv_suffix not in csv_name:  # 跳过不是.xlsx后缀的文件
            continue

        csv_path = os.path.join(csv_dir_path, csv_name)    # 计算.csv路径
        xlsx_name = csv_name.replace(csv_suffix, xlsx_suffix)
        xlsx_path = os.path.join(xlsx_dir_path, xlsx_name) # 计算.xlsx路径

        csv = pd.read_csv(csv_path, encoding='utf-8')
        csv.to_excel(xlsx_path, sheet_name='data') 

# 将csv文件格式化为json
def csv_to_json(csv_dir_path, json_dir_path):
    csv_suffix = '.csv'
    json_suffix = '.json'
    config_json_suffix = '_type.json'

    dir_list = os.listdir(csv_dir_path)  # dir_list中记录了所有的文件名 
    for csv_name in dir_list:
        if csv_suffix not in csv_name:  # 跳过不是.xlsx后缀的文件
            continue

        csv_path = os.path.join(csv_dir_path, csv_name)    # 计算.csv路径
        json_name = csv_name.replace(csv_suffix, json_suffix)
        json_path = os.path.join(json_dir_path, json_name) # 计算.xlsx路径
        config_json_name = csv_name.replace(csv_suffix, config_json_suffix)
        config_json_path = os.path.join(json_dir_path, config_json_name)

        f = open(csv_path, 'r', encoding='utf-8')
        
        # 预处理，删除所有\n，记录名称和类型
        data_list, type_list = csv_dic_formatter(f)
        # save_to_json(json_path, data_list, 'data')
        # save_to_json(config_json_path, type_list, 'type')
        save_both_to_json(json_path, data_list, type_list)

# 类型转换器，根据传入的类型输出转换后的类型（可以后期扩展）
def type_converter(type_str, str):
    if type_str == "int":
        return int(str)
    elif type_str == 'float':
        return float(str)
    return str

# 预处理，删除所有\n，记录名称和类型
def csv_dic_formatter(f):
    name_list = []
    type_list = []
    data_list = []
    ignore_first_line = True 

    for line in f:
        if ignore_first_line:
            ignore_first_line = False
            continue

        row = line.split(',')
        temp_str = row[len(row) - 1]
        row[len(row) - 1] = temp_str[0:len(temp_str) - 1]
        if len(name_list) == 0:
            name_list = row
        elif len(type_list) == 0:
            type_list = row
        else:
            temp_dic = {}
            for i in range(0, len(row)):
                value = type_converter(type_list[i], row[i])
                temp_dic[name_list[i]] = value
            id = type_converter(type_list[0], row[0])
            data_list.append(temp_dic)

    type_list_new = []
    for i in range(0, len(type_list)):
        temp = {}
        temp['type'] = type_list[i]
        temp['name'] = name_list[i]
        type_list_new.append(temp)

    return data_list, type_list_new

# 保存为json文件
def save_to_json(json_path, data_list, json_name):
    dir = {}
    dir[json_name] = data_list
    with open(json_path, 'w') as f:
        json.dump(dir, f, indent=4)

# 保存为json文件
def save_both_to_json(json_path, data_list, type_list):
    dir = {}
    dir['type'] = type_list
    dir['data'] = data_list
    with open(json_path, 'w') as f:
        json.dump(dir, f, indent=4)
 
if __name__ == '__main__':
    xlsx_dir_path = './XLSXPath'
    csv_dir_path = './CSVPath'
    json_dir_path = './JSONPath'
    xlsx_to_csv_pd(xlsx_dir_path, csv_dir_path)
    csv_to_json(csv_dir_path, json_dir_path)

