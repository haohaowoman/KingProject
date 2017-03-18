// 下列 ifdef 块是创建使从 DLL 导出更简单的
// 宏的标准方法。此 DLL 中的所有文件都是用命令行上定义的 RS485CTRL_EXPORTS
// 符号编译的。在使用此 DLL 的
// 任何其他项目上不应定义此符号。这样，源文件中包含此文件的任何其他项目都会将
// RS485CTRL_API 函数视为是从 DLL 导入的，而此 DLL 则将用此宏定义的
// 符号视为是被导出的。
#ifdef RS485CTRL_EXPORTS
#define RS485CTRL_API __declspec(dllexport)
#else
#define RS485CTRL_API __declspec(dllimport)
#endif


// 下列 ifdef 块是创建使从 DLL 导出更简单的
// 宏的标准方法。此 DLL 中的所有文件都是用命令行上定义的 RS485CTRL_EXPORTS
// 符号编译的。在使用此 DLL 的
// 任何其他项目上不应定义此符号。这样，源文件中包含此文件的任何其他项目都会将
// RS485CTRL_API 函数视为是从 DLL 导入的，而此 DLL 则将用此宏定义的
// 符号视为是被导出的。
#ifdef RS485CTRL_EXPORTS
#define RS485CTRL_API __declspec(dllexport)
#else
#define RS485CTRL_API __declspec(dllimport)
#endif

struct RunState
{
	bool RDY_ON;		//ready to switch on
	bool RDY_RUN;		//ready to operate
	bool RDY_REF;		//operation enabled
	bool TRIPPED;		//fault
	bool OFF_2_STATUS;	//OFF2失效
	bool OFF_3_STATUS;	//OFF3失效
	bool SWC_ON_INHIB;	//switch-on inhibited
	bool ALARM;			//警告/报警
	bool AT_SETPOINT;	//operating 实际值等于给定值（在容限范围内）
	bool REMOTE;		//传动控制地  
	bool ABOVE_LIMIT;	//实际频率或速度等于或超出监控限值
};

#ifdef __cplusplus
extern "C"
{
#endif	

	RS485CTRL_API BOOL				InitSerial(LPTSTR com);//设置COM的参数，nCom是COM口，如COM1，comData是参数，如“9600，N，8，1”
	RS485CTRL_API bool				SetCRC(bool bHighBefore,bool bPoly,unsigned short poly);//设置CRC校验，bHighBefore表示校验位高位在前，bPoly是否采用多项式CRC校验，poly是CRC校验码
	RS485CTRL_API void				SetAddr(char addr);

	RS485CTRL_API BOOL				SetRevolveRate(unsigned int rate);	//设置转速
	RS485CTRL_API bool				SetTorque(unsigned int rate);		//设置转矩
	RS485CTRL_API BOOL              SetFJInit();                        //风机初始化
	RS485CTRL_API BOOL				SetStart();							//风机启动
	RS485CTRL_API BOOL				SetStop();							//风机停止
	RS485CTRL_API BOOL              SetRest();                          //风机复位
	RS485CTRL_API bool				SetHZ(float rate);			        //设置频率
	RS485CTRL_API int				GetHZ();							//获取频率

	RS485CTRL_API unsigned char		GetLastErrorCode();					//获取错误码
	RS485CTRL_API BOOL				GetConnectState();					//获取连接状态
	RS485CTRL_API int				GetRunState();						//获取运行状态,返回-1表示运行停止，返回0表示正常运行
	RS485CTRL_API short				GetNowTemperRate();				    //获取温度
	RS485CTRL_API short             GetNowSpeed();                      //获取电机转速
	RS485CTRL_API short             GetNowCurrent();                    //获取电机电流
	RS485CTRL_API short             GetNowVoltage();                    //获取电机电压
	RS485CTRL_API short				GetNowTorque();						//获取当前的转矩
	RS485CTRL_API void				ReleaseData();						//释放资源、关闭串口、断开连接等
	RS485CTRL_API const float*      GetVlue();                          //z获取对应的资源的值
	RS485CTRL_API bool              paramTranslate(unsigned short addr,unsigned short funCode);  //z用于参数传递的中间函数
	RS485CTRL_API bool              OnWrite(unsigned short addr,unsigned short funCode);      //z用于放到线程中不断设置通讯不中断的函数

	RS485CTRL_API DWORD  WINAPI     ThreadProc(LPVOID lpParameter);       //z线程回调函数
	RS485CTRL_API bool              AllRelease();                         
	RS485CTRL_API bool              AllReady();
	RS485CTRL_API void              InitMyFirstThread();
	RS485CTRL_API void              ExitMyThread();
	RS485CTRL_API short             ErroMessage();
	RS485CTRL_API bool              CommunicationReady();

	RS485CTRL_API LPCTSTR			GetErroExplain(short code);
	RS485CTRL_API void              erroAutoKill(int f);
	RS485CTRL_API void              ProtectPrgam(float b,unsigned short gnm);

#ifdef __cplusplus
};
#endif // __cplusplus



