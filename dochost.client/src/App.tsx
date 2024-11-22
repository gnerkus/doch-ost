import './App.css';
import {QueryClient, QueryClientProvider} from "@tanstack/react-query";
import AuthProvider from "./auth/AuthContext.tsx";
import AppRoutes from "./layout/AppRoutes.tsx";

const queryClient = new QueryClient();

function App() {

    return (
        <AuthProvider>
            <QueryClientProvider client={queryClient}>
                <AppRoutes />
            </QueryClientProvider>
        </AuthProvider>
    )
}

export default App;