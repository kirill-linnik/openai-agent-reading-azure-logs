import { Alert } from "antd";
import { FC, useEffect } from "react";
import { useTranslation } from "react-i18next";
import { useAppDispatch } from "../store";
import { COMMON_REMOVE_INFO } from "../store/types/commonTypes";

type InfoBlockProps = {
  info: string | undefined | null;
};

export const InfoBlock: FC<InfoBlockProps> = ({ info }) => {
  const { t } = useTranslation();
  const dispatch = useAppDispatch();

  const delay: number = 4500;
  useEffect(() => {
    if (info) {
      setTimer(delay);
    }
  }, [info]);

  const setTimer = (delay: number) => {
    setTimeout(() => dispatch({ type: COMMON_REMOVE_INFO }), delay);
  };

  if (info) {
    return (
      <>
        <Alert
          type="info"
          showIcon
          closable
          message={t("info")}
          description={t(info)}
        />
        <br />
      </>
    );
  } else {
    return <></>;
  }
};
