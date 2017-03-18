// ���� ifdef ���Ǵ���ʹ�� DLL �������򵥵�
// ��ı�׼�������� DLL �е������ļ��������������϶���� RS485CTRL_EXPORTS
// ���ű���ġ���ʹ�ô� DLL ��
// �κ�������Ŀ�ϲ�Ӧ����˷��š�������Դ�ļ��а������ļ����κ�������Ŀ���Ὣ
// RS485CTRL_API ������Ϊ�Ǵ� DLL ����ģ����� DLL ���ô˺궨���
// ������Ϊ�Ǳ������ġ�
#ifdef RS485CTRL_EXPORTS
#define RS485CTRL_API __declspec(dllexport)
#else
#define RS485CTRL_API __declspec(dllimport)
#endif


// ���� ifdef ���Ǵ���ʹ�� DLL �������򵥵�
// ��ı�׼�������� DLL �е������ļ��������������϶���� RS485CTRL_EXPORTS
// ���ű���ġ���ʹ�ô� DLL ��
// �κ�������Ŀ�ϲ�Ӧ����˷��š�������Դ�ļ��а������ļ����κ�������Ŀ���Ὣ
// RS485CTRL_API ������Ϊ�Ǵ� DLL ����ģ����� DLL ���ô˺궨���
// ������Ϊ�Ǳ������ġ�
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
	bool OFF_2_STATUS;	//OFF2ʧЧ
	bool OFF_3_STATUS;	//OFF3ʧЧ
	bool SWC_ON_INHIB;	//switch-on inhibited
	bool ALARM;			//����/����
	bool AT_SETPOINT;	//operating ʵ��ֵ���ڸ���ֵ�������޷�Χ�ڣ�
	bool REMOTE;		//�������Ƶ�  
	bool ABOVE_LIMIT;	//ʵ��Ƶ�ʻ��ٶȵ��ڻ򳬳������ֵ
};

#ifdef __cplusplus
extern "C"
{
#endif	

	RS485CTRL_API BOOL				InitSerial(LPTSTR com);//����COM�Ĳ�����nCom��COM�ڣ���COM1��comData�ǲ������硰9600��N��8��1��
	RS485CTRL_API bool				SetCRC(bool bHighBefore,bool bPoly,unsigned short poly);//����CRCУ�飬bHighBefore��ʾУ��λ��λ��ǰ��bPoly�Ƿ���ö���ʽCRCУ�飬poly��CRCУ����
	RS485CTRL_API void				SetAddr(char addr);

	RS485CTRL_API BOOL				SetRevolveRate(unsigned int rate);	//����ת��
	RS485CTRL_API bool				SetTorque(unsigned int rate);		//����ת��
	RS485CTRL_API BOOL              SetFJInit();                        //�����ʼ��
	RS485CTRL_API BOOL				SetStart();							//�������
	RS485CTRL_API BOOL				SetStop();							//���ֹͣ
	RS485CTRL_API BOOL              SetRest();                          //�����λ
	RS485CTRL_API bool				SetHZ(float rate);			        //����Ƶ��
	RS485CTRL_API int				GetHZ();							//��ȡƵ��

	RS485CTRL_API unsigned char		GetLastErrorCode();					//��ȡ������
	RS485CTRL_API BOOL				GetConnectState();					//��ȡ����״̬
	RS485CTRL_API int				GetRunState();						//��ȡ����״̬,����-1��ʾ����ֹͣ������0��ʾ��������
	RS485CTRL_API short				GetNowTemperRate();				    //��ȡ�¶�
	RS485CTRL_API short             GetNowSpeed();                      //��ȡ���ת��
	RS485CTRL_API short             GetNowCurrent();                    //��ȡ�������
	RS485CTRL_API short             GetNowVoltage();                    //��ȡ�����ѹ
	RS485CTRL_API short				GetNowTorque();						//��ȡ��ǰ��ת��
	RS485CTRL_API void				ReleaseData();						//�ͷ���Դ���رմ��ڡ��Ͽ����ӵ�
	RS485CTRL_API const float*      GetVlue();                          //z��ȡ��Ӧ����Դ��ֵ
	RS485CTRL_API bool              paramTranslate(unsigned short addr,unsigned short funCode);  //z���ڲ������ݵ��м亯��
	RS485CTRL_API bool              OnWrite(unsigned short addr,unsigned short funCode);      //z���ڷŵ��߳��в�������ͨѶ���жϵĺ���

	RS485CTRL_API DWORD  WINAPI     ThreadProc(LPVOID lpParameter);       //z�̻߳ص�����
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



