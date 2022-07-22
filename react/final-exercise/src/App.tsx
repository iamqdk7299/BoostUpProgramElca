import { AppBar } from '@material-ui/core';
import React from 'react';
import { QueryClient, QueryClientProvider } from 'react-query';
import { BrowserRouter, Redirect, Route, Switch } from 'react-router-dom';
import TabRouteNavs, { CustomTab } from './common/components/TabRoutesNav';
import ProjectList from './screens/ProjectList/ProjectList';

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      refetchOnWindowFocus: false,
    },
  },
});

const MainTabs: CustomTab[] = [
  {
    key: 'home',
    title: 'Home',
    href: '/home',
  },
  {
    key: 'project-list',
    title: 'Project List',
    href: '/project-list',
  },
];

function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <BrowserRouter>
        <main>
          <AppBar position='static'>
            <TabRouteNavs tabs={MainTabs} />
          </AppBar>
          <Switch>
            <Route path='/project-list'>
              <ProjectList />
            </Route>
            <Route path='/home' exact>
              Home
            </Route>
            <Route path='/'>
              <Redirect to='/home' />
            </Route>
          </Switch>
        </main>
      </BrowserRouter>
    </QueryClientProvider>
  );
}

export default App;
