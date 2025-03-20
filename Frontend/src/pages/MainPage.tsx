import { SendOutlined } from "@ant-design/icons";
import { Button, DatePicker, Form, Input } from "antd";
import { FC, ReactNode, useEffect } from "react";
import ReactMarkdown from "react-markdown";
import { ChatRole } from "../models/ChatRole";
import { useAppDispatch, useAppSelector } from "../store";
import { addChatMessage, createChatThread, fetchFreshMessages } from "../store/actions/chatActions";
import "./ChatStyles.css";

const { RangePicker } = DatePicker;

const MainPage: FC = () => {
  const dispatch = useAppDispatch();
  const chat = useAppSelector((state) => state.chat);

  const [form] = Form.useForm();

  useEffect(() => {
    if (!chat.threadId) {
      dispatch(createChatThread());
    }
  }, [chat.threadId, dispatch]);

  useEffect(() => {
    if (chat.threadId) {
      const interval = setInterval(() => {
        const lastMessageId =
          chat.history.length > 0 ? chat.history[chat.history.length - 1].id : undefined;
        dispatch(fetchFreshMessages(chat.threadId!, lastMessageId));
      }, 500);
      return () => clearInterval(interval);
    }
  }, [chat.threadId, chat.history, dispatch]);

  const displayChatHistory = (): ReactNode => {
    const chatBubbles = chat.history.map((message, index) => {
      return (
        <div key={index}>
          {message.role === ChatRole.USER ? (
            <div className="outgoing-chats">
              <div className="outgoing-msg">
                <div className="outgoing-chats-msg">
                  <p>{message.content}</p>
                </div>
              </div>
            </div>
          ) : (
            <div className="received-chats">
              <div className="received-msg">
                <div className="received-msg-inbox">
                  <p className="text">
                    <ReactMarkdown>{message.content}</ReactMarkdown>
                  </p>
                </div>
              </div>
            </div>
          )}
        </div>
      );
    });
    return (
      <div className="chats">
        <div className="msg-page">{chatBubbles}</div>
      </div>
    );
  };

  return (
    <div className="container">
      ACS chat thread id: <strong>{chat.threadId}</strong>
      <br />
      {displayChatHistory()}
      <div className="msg-bottom">
        <div className="input-group">
          <Form
            name="chatForm"
            initialValues={{
              layout: "inline",
            }}
            form={form}
            layout="inline"
            onFinish={(values) => {
              dispatch(addChatMessage(chat.threadId!, values.text));
              form.resetFields();
            }}
          >
            <Form.Item
              name="text"
              rules={[
                {
                  required: true,
                },
              ]}
              style={{ width: "400px" }}
            >
              <Input placeholder="Type a message" />
            </Form.Item>
            <Form.Item>
              <Button type="primary" disabled={chat.threadId === undefined} htmlType="submit">
                <SendOutlined />
              </Button>
            </Form.Item>
          </Form>
        </div>
      </div>
    </div>
  );
};

export default MainPage;
