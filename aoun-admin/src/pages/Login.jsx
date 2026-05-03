import { useState } from 'react';
import axios from 'axios';

export default function Login({ setAuth }) {
    const [email, setEmail] = useState('admin@test.com');
    const [password, setPassword] = useState('Admin123!');
    const [error, setError] = useState('');
    const [loading, setLoading] = useState(false);

    const handleLogin = async (e) => {
        e.preventDefault();
        setLoading(true); setError('');
        try {
            const res = await axios.post('http://localhost:5055/api/Auth/login', { email, password });
            if (res.data && res.data.token) {
                localStorage.setItem('admin_token', res.data.token);
                setAuth(true);
            }
        } catch (err) {
            setError(err.response?.data?.message || 'بيانات الدخول غير صحيحة');
        } finally { setLoading(false); }
    };

    return (
        <div className="min-h-screen flex items-center justify-center bg-gray-900">
            <div className="bg-white p-8 rounded-xl shadow-2xl w-96 text-center">
                <div className="mb-6">
                    <h2 className="text-3xl font-bold text-teal-600 mb-2">منصة عون</h2>
                    <p className="text-gray-500 text-sm">لوحة تحكم الإدارة العليا</p>
                </div>
                <form onSubmit={handleLogin} className="space-y-4 text-right">
                    <div>
                        <label className="block text-sm font-semibold mb-1">البريد الإلكتروني</label>
                        <input type="email" value={email} onChange={e => setEmail(e.target.value)} className="w-full px-4 py-2 border rounded-lg focus:ring-teal-500 text-left" dir="ltr" required />
                    </div>
                    <div>
                        <label className="block text-sm font-semibold mb-1">كلمة المرور</label>
                        <input type="password" value={password} onChange={e => setPassword(e.target.value)} className="w-full px-4 py-2 border rounded-lg focus:ring-teal-500 text-left" dir="ltr" required />
                    </div>
                    {error && <p className="text-red-500 text-sm text-center">{error}</p>}
                    <button type="submit" disabled={loading} className="w-full bg-teal-600 hover:bg-teal-700 text-white font-bold py-3 rounded-lg transition-colors">
                        {loading ? 'جاري الدخول...' : 'تسجيل الدخول'}
                    </button>
                </form>
            </div>
        </div>
    );
}