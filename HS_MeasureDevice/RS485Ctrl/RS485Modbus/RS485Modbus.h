#ifdef RS485MODBUS_EXPORTS
#define RS485MODBUS_API __declspec(dllexport)
#else
#define RS485MODBUS_API __declspec(dllimport)
#endif

struct SendSeal16
{
	unsigned char functionCode;	//������
	unsigned short jcqAddr;		//��ʼ��ַ/�����ַ/�Ĵ�����ַ
	unsigned short counts;		//��Ȧ��/����/�Ĵ�����/���ֵ/�Ĵ���ֵ
	unsigned char bytes;		//�ֽ���
	unsigned char* outData;		//���ֵ
	unsigned short* jcqData;	//�Ĵ���ֵ
	unsigned int dataLen;		//outData����jcqData�����ݸ���
	unsigned char CRCCode[2];	//CRCУ����

	bool bHighBefore;			//CRC���λ��ǰ
	bool bPolynomail;			//���ö���ʽ����
	unsigned short polynomail;  //����ʽУ���룬Ĭ��0xa001

	unsigned char* p_seal;		//Э�������
	unsigned int len;			//Э��ĳ���

	unsigned char recvFuncCode; //���յĹ�����
	unsigned char recvBytes;	//�ֽ���
	unsigned char* state;		//��Ȧ״̬/���״̬
	unsigned short* data;		//�Ĵ���ֵ/����Ĵ���
	unsigned int recvDataLen;	//����state����data�����ݸ���
	unsigned short recvOutAddr;	//�����ַ/�Ĵ�����ַ/��ʼ��ַ
	unsigned short recvData;	//���ֵ/�Ĵ���ֵ/�������/�Ĵ�������

	bool bError;				//��ǽ��յ���Ӧ�Ƿ��쳣

	unsigned char errorFuncCode;//�쳣������
	unsigned char errorCode;	//�쳣��  01-�Ƿ����� 02-�Ƿ����ݵ�ַ 03-�Ƿ�����ֵ 04-��վ�豸���� 05-ȷ�� 06-�����豸æ 08-�洢��ż�Բ�� 0A-����������·�� 0B-����Ŀ���豸��Ӧʧ��
};

#ifdef __cplusplus
extern "C"
{
#endif // __cplusplus

	RS485MODBUS_API bool GetSealCommand(SendSeal16* m_seal);
	RS485MODBUS_API void AnalysisRecvSeal(unsigned char* recv,unsigned int recvLen,SendSeal16* m_seal);

	//m_seal-Э��ṹ�壬functionCode-�����룬jcqAddr-�Ĵ�����ַ��counts-������bytes-�ֽ�����outData-������ݣ�jcqData-�Ĵ���ֵ��dataLen-������ݻ�Ĵ���ֵ���ݸ�����bHighBefore-��λ��ǰ��bPolynomail-CRC��У�飬polynomail-CRCУ����
	RS485MODBUS_API bool InitSeal(SendSeal16* m_seal,unsigned char functionCode,unsigned short jcqAddr,unsigned short counts,unsigned char bytes = 0x00,unsigned char* outData = NULL,unsigned short* jcqData = NULL,unsigned int dataLen = 0,bool bHighBefore = false,bool bPolynomail = true,unsigned short polynomail = 0xa001);

	RS485MODBUS_API unsigned int GetSealLen(SendSeal16* m_seal);
	RS485MODBUS_API void SetHighBefore(SendSeal16* m_seal,bool bBefore);
	RS485MODBUS_API bool GetHighBefore(SendSeal16* m_seal);
	RS485MODBUS_API void SetBPolyNomail(SendSeal16* m_seal,bool bPoly);
	RS485MODBUS_API void SetFjAddr(char addr);
	RS485MODBUS_API bool GetBPolyNomail(SendSeal16* m_seal);
	RS485MODBUS_API void SetNomail(SendSeal16* m_seal,unsigned short bNomail);
	RS485MODBUS_API unsigned short GetNomail(SendSeal16* m_seal);
	RS485MODBUS_API void FreeSeal(SendSeal16* m_seal);
#ifdef __cplusplus
};
#endif