# funnel
funnel 是一个用于从大文件中提取指定信息的工具

## Configuration Example
```
Inputs:
  - /var/log/xxx/xxx.log
Encoding: GB2312
Limitation: 100000
Multiline: true
LineHead: ^{
Keywords:
  - asdfasdfasdfasd
Outputs:
  - console
  - file:/var/log/fetcher.txt
```

- Inputs即为需要查询的日志文件列表
- Encoding为日志文件的编码格式，为空或者不设置会使用默认编码格式，可能会导致中文乱码
- Limitation为每秒钟读取文本行数，这个配置主要用于限速，避免程序影响到机器性能，当Limitation大于0时，会对Inputs顺序处理，否则会对Inputs并行处理，并且不会限速
- Multiline主要用于多行日志块。当一条日志只有一行时，此配置设为false即可。当一条日志跨越多行，比如java exception日志，此时可将此配置设置为true，并且设置LineHead配置，即可将多行日志识别为一个日志块。LineHead支持正则表达式
    ```
    日志：
    {
        "Data": {
        }
    }

    配置：
    Multiline: true
    LineHead: ^{

    输出日志块：
    {
        "Data": {
        }
    }
    ```
- Keywords为想要查询的文本信息，列表中项与项之间的关系为or，若需要实现and关系，可以使用正则表达式，比如：
    ```
    Keywords:
      - aaaa.*bbbb
      - bbbb.*aaaa
    ```
- Outputs为查询到的日志信息的输出方式，目前支持console和file两种方式，file后面紧跟一个英文冒号以及文件目录

### 运行命令：
dotnet run funnel.yml

### 或下载 release 包，运行：
funnel funnel.yml
