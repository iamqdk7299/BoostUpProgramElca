import { Tabs } from "@material-ui/core";
import Tab from "@material-ui/core/Tab";
import React, { useEffect } from "react";
import { Link, useLocation } from "react-router-dom";

export type TabRouteNavsProps = {
  tabs: CustomTab[],
  customRouterLink?: React.ElementType,
  customStyle?: string
};

export type CustomTab = {
  key: string;
  title: string;
  href: string;
  tabMatchResolver?: (locationPathname: string) => boolean;
};

const TabRouteNavs = (props: React.PropsWithChildren<TabRouteNavsProps>): JSX.Element => {
  const { tabs,
    customRouterLink,
    customStyle
  } = props;

  const location = useLocation();
  const matchedIndex = tabs.findIndex(
    (t) =>
      location.pathname.includes(t.href) ||
      (t.tabMatchResolver && t.tabMatchResolver(location.pathname))
  );

  const [value, setValue] = React.useState(matchedIndex);

  useEffect(() => {
    setValue(matchedIndex);
  }, [matchedIndex]);
  
  const onChangeHandler = (_: React.ChangeEvent<unknown>, newValue: number) => {
    setValue(newValue);
  };

  return (
    <Tabs
      value={value}
      indicatorColor="primary"
      onChange={onChangeHandler}
      className={customStyle}
    >
      {
        tabs.map((tab, index) => (
          <Tab
            key={tab.key}
            label={tab.title}
            to={tab.href}
            component={customRouterLink ? customRouterLink : Link}
            value={index}
          />
        ))
      }
    </Tabs>
  );
};

export default TabRouteNavs;
