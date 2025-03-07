import "@ant-design/v5-patch-for-react-19";
import { Layout, Menu, theme } from "antd";
import { FC, lazy } from "react";
import { BrowserRouter, Route, Routes } from "react-router-dom";
import "./App.css";

const { Header, Content, Footer } = Layout;

const MainPage = lazy(() => import("./pages/MainPage"));
const NotFoundPage = lazy(() => import("./pages/NotFoundPage"));

const App: FC = () => {
  const {
    token: { colorBgContainer, borderRadiusLG },
  } = theme.useToken();
  return (
    <>
      <Layout>
        <Header style={{ display: "flex", alignItems: "center" }}>
          <div className="demo-logo" />
          <Menu
            theme="dark"
            mode="horizontal"
            defaultSelectedKeys={["chat"]}
            items={[{ key: "chat", label: "chat" }]}
            style={{ flex: 1, minWidth: 0 }}
          />
        </Header>
        <Content style={{ padding: "0 48px" }}>
          <div
            style={{
              background: colorBgContainer,
              minHeight: "calc(100vh - 64px)",
              padding: 24,
              borderRadius: borderRadiusLG,
            }}
          >
            <BrowserRouter>
              <div className="page-content">
                <Routes>
                  <Route path="/" element={<MainPage />} />
                  <Route path="*" element={<NotFoundPage />} />
                </Routes>
              </div>
            </BrowserRouter>
          </div>
        </Content>
      </Layout>
    </>
  );
};

export default App;
